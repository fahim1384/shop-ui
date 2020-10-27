using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HandiCrafts.Core.Exceptions;
using HandiCrafts.Web.Infrastructure.Security;
using HandiCrafts.Web.Interfaces;
using HandiCrafts.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace HandiCrafts.Web.Infrastructure.Framework
{
    public class BaseController : Controller
    {
        private const string SUCCESS = "success";
        private const string ERROR = "error";

        protected ILogger Logger { get; private set; }

        protected IStringLocalizer<SharedResource> Localizer { get; private set; }
        protected IMapper Mapper { get; private set; }

        public BaseController(ILogger logger, IStringLocalizer<SharedResource> localizer, IMapper mapper)
        {
            Logger = logger;
            Localizer = localizer;
            Mapper = mapper;
        }


        /// <summary>
        /// Display success notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        protected virtual void NotifySuccess(string message, bool persistForTheNextRequest = true)
        {
            AddNotification(SUCCESS, message, persistForTheNextRequest);
        }
        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        protected virtual void NotifyError(string message, bool persistForTheNextRequest = true)
        {
            AddNotification(ERROR, message, persistForTheNextRequest);
        }
        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        /// <param name="logException">A value indicating whether exception should be logged</param>
        protected virtual void NotifyError(Exception exception, bool persistForTheNextRequest = true)
        {
            AddNotification(ERROR, exception.Message, persistForTheNextRequest);
        }
        /// <summary>
        /// Display notification
        /// </summary>
        /// <param name="type">Notification type</param>
        /// <param name="message">Message</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        protected virtual void AddNotification(string type, string message, bool persistForTheNextRequest)
        {
            string dataKey = string.Format("nk.notify.{0}", type);
            if (persistForTheNextRequest)
            {
                if (TempData[dataKey] == null)
                {
                    TempData[dataKey] = new List<string>();
                }

                if (TempData[dataKey] is string[])
                {
                    var arr = TempData[dataKey] as string[];
                    var list = new List<string>(arr);
                    TempData[dataKey] = list;
                }

                ((List<string>)TempData[dataKey]).Add(message);
            }
            else
            {
                if (ViewData[dataKey] == null)
                {
                    ViewData[dataKey] = new List<string>();
                }

                if (ViewData[dataKey] is string[])
                {
                    var arr = ViewData[dataKey] as string[];
                    var list = new List<string>(arr);
                    ViewData[dataKey] = list;
                }

                ((List<string>)ViewData[dataKey]).Add(message);
            }
        }
        protected DefaultResponseState Success(string message = "")
        {
            return new DefaultResponseState
            {
                Success = true,
                Message = message,
                MessageType = ResponseStateMessageTypes.Success,
            };
        }

        protected DefaultResponseState Error(string message = "")
        {
            return new DefaultResponseState
            {
                Success = false,
                Message = message,
                MessageType = ResponseStateMessageTypes.Error,
            };
        }

        protected DefaultResponseState Info(string message = "")
        {
            return new DefaultResponseState
            {
                Success = true,
                Message = message,
                MessageType = ResponseStateMessageTypes.Info,
            };
        }

        protected DefaultResponseState Warning(string message = "")
        {
            return new DefaultResponseState
            {
                Success = true,
                Message = message,
                MessageType = ResponseStateMessageTypes.Warning,
            };
        }

        protected ResponseState<T> Success<T>(T data = default(T), string message = "")
        {
            return MakeResponse(true, message, ResponseStateMessageTypes.Success, data);
        }

        protected ResponseState<T> Error<T>(T data = default(T), string message = "")
        {
            return MakeResponse(false, message, ResponseStateMessageTypes.Error, data);
        }

        protected ResponseState<T> Info<T>(T data = default(T), string message = "")
        {
            return MakeResponse(true, message, ResponseStateMessageTypes.Info, data);
        }

        protected ResponseState<T> Warning<T>(T data = default(T), string message = "")
        {
            return MakeResponse(true, message, ResponseStateMessageTypes.Warning, data);
        }

        protected ResponseState<T> MakeResponse<T>(bool success, string message, ResponseStateMessageTypes messageType, T data)
        {
            return new ResponseState<T>
            {
                Success = success,
                Message = message,
                MessageType = messageType,
                Data = data,
            };
        }

        protected async Task<ResponseState<T>> TryCatch<T>(Func<Task<ResponseState<T>>> func, bool addModelState = false)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var error = JoinErrors();
                    return Error<T>(message: error);
                }

                return await func();
            }
            catch (HandiCraftsException ex)
            {
                var msg = HandleException(ex, addModelState);
                return Error<T>(message: msg);
            }
            catch (Exception ex)
            {
                //var msg = HandleException(ex, addModelState);
                return Error<T>(message: ex.Message);
            }
        }

        protected async Task<DefaultResponseState> TryCatch(Func<Task<DefaultResponseState>> func, bool addModelState = false)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var error = JoinErrors();
                    return Error(message: error);
                }

                return await func();
            }
            catch (HandiCraftsException ex)
            {
                var msg = HandleException(ex, addModelState);
                return Error(message: msg);
            }
            catch (Exception ex)
            {
                //var msg = HandleException(ex, addModelState);
                return Error(message: ex.Message);
            }
        }
        protected string JoinErrors()
        {
            var errors = new Dictionary<string, List<string>>();

            if (!ModelState.IsValid)
            {
                if (ModelState.ErrorCount > 0)
                {
                    var invalids = ModelState.Values.Where(x => x.ValidationState == ModelValidationState.Invalid).ToArray();

                    for (int i = 0; i < ModelState.Values.Count(); i++)
                    {
                        var key = ModelState.Keys.ElementAt(i);
                        var value = ModelState.Values.ElementAt(i);

                        if (value.ValidationState == ModelValidationState.Invalid)
                        {
                            errors.Add(key, value.Errors.Select(x => string.IsNullOrEmpty(x.ErrorMessage) ? x.Exception?.Message : x.ErrorMessage).ToList());
                        }
                    }
                }
            }

            var error = string.Join("<br/>", errors.Select(x =>
            {
                return $"{string.Join(" - ", x.Value)}";

                // return Localizer["msg.join-errors", x.Key, $"({string.Join(") - (", x.Value)})"].Value;
            }));
            return error;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        protected async Task<string> PartialViewToString(PartialViewResult partialView, ActionContext actionContext)
        {
            using (var writer = new StringWriter())
            {
                var services = actionContext.HttpContext.RequestServices;
                var executor = services.GetRequiredService<PartialViewResultExecutor>();
                var view = executor.FindView(actionContext, partialView).View;
                var viewContext = new ViewContext(actionContext, view, partialView.ViewData, partialView.TempData, writer, new HtmlHelperOptions());
                await view.RenderAsync(viewContext);
                return writer.ToString();
            }
        }
        #region Private Methods
        private void HandleExceptionRelatedErrors(HandiCraftsException ex, ref string userMsg)
        {
            if (ex.RelatedErrors != null && ex.RelatedErrors.Any())
            {
                userMsg += "<br/>";
                foreach (var err in ex.RelatedErrors)
                {
                    userMsg += "- " + Localizer[err].Value + "<br/>";
                }
            }
        }
        
        private string HandleException(HandiCraftsException exception, bool addModelState)
        {
            var userMsg = exception.Arguments == null ? Localizer[exception.Message].Value : Localizer[exception.Message, exception.Arguments];
            HandleExceptionRelatedErrors(exception, ref userMsg);

            Logger.LogError(exception, exception.Message);
            if (addModelState)
            {
                ModelState.AddModelError("err", userMsg);
            }
            return userMsg;
        }
        //private string HandleException(Exception exception, bool addModelState)
        //{
        //    var userMsg = Localizer["msg.invalid-request"].Value;
        //    if (exception.InnerException != null)
        //    {
        //        var sqlException = exception.InnerException as SqlException;
        //        if (sqlException.Number == 2601 || sqlException.Number == 2627)
        //        {
        //            userMsg = Localizer["msg.duplicate-request"].Value;
        //        }
        //    }

        //    Logger.LogError(exception, exception.Message);
        //    if (addModelState)
        //    {
        //        ModelState.AddModelError("err", userMsg);
        //    }

        //    return userMsg;
        //}

        #endregion

    }
}
