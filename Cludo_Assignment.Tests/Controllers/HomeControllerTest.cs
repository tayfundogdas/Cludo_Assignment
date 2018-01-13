using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cludo_Assignment;
using Cludo_Assignment.Controllers;
using Cludo_Assignment.Models;
using System.Web;
using System.Web.Routing;
using System.Collections.Specialized;
using Moq;
using System.Security.Principal;
using Cludo_Assignment.Helpers;
using Lucene.Net.Store;
using Tweetinvi;
using Business;

namespace Cludo_Assignment.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        private HttpContextBase GetMockedHttpContext()
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            //init new ram dir for testing
            session.SetupGet(s => s[SessionHelper.INDEX_SESSION_NAME]).Returns(new RAMDirectory()); //somevalue
            var server = new Mock<HttpServerUtilityBase>();
            var user = new Mock<IPrincipal>();
            var identity = new Mock<IIdentity>();
            var urlHelper = new Mock<UrlHelper>();

            //var routes = new RouteCollection();
            //MvcApplication.RegisterRoutes(routes);
            var requestContext = new Mock<RequestContext>();
            requestContext.Setup(x => x.HttpContext).Returns(context.Object);
            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);
            context.Setup(ctx => ctx.User).Returns(user.Object);
            user.Setup(ctx => ctx.Identity).Returns(identity.Object);
            identity.Setup(id => id.IsAuthenticated).Returns(true);
            identity.Setup(id => id.Name).Returns("test");
            request.Setup(req => req.Url).Returns(new Uri("http://www.google.com"));
            request.Setup(req => req.RequestContext).Returns(requestContext.Object);
            requestContext.Setup(x => x.RouteData).Returns(new RouteData());
            request.SetupGet(req => req.Headers).Returns(new NameValueCollection());

            return context.Object;
        }

        [TestInitialize]
        public void Init()
        {
            //set mocks
            HttpContextManager.SetCurrentContext(GetMockedHttpContext());

            //set twitter credentials
            // If you do not already have a BearerToken, use the TRUE parameter to automatically generate it.
            // Note that this will result in a WebRequest to be performed and you will therefore need to make this code safe
            var appCreds = Auth.SetApplicationOnlyCredentials("MvQXCZIKKondIuS8FHuRWVlgJ", "S2k8jrBuZOGbM3mxogZgpJyuoQRZuZZUDRHT6jcZHkObVry5Bm", true);
        }

        [TestMethod]
        public void TestTwitterSearch()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            var model = new TwitterSearch
            {
                HashTag = "tayfun"
            };
            ViewResult result = controller.Index(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result.Model as TwitterSearch);
        }

        [TestMethod]
        public void TestFilterSearchResult()
        {
            // Arrange
            HomeController controller = new HomeController();

            //prepare mocked index
            TwitterFlow flow = new TwitterFlow(SessionHelper.GetIndex());
            var searchResult = flow.SearchHashTagOnTwitter();

            flow.IndexSearchResult(searchResult);

            // Act
            var model = new TwitterSearch
            {
                Filter = "tazminat"
            };
            ViewResult result = controller.Index(model) as ViewResult;

            // Assert
            Assert.AreEqual(2, (result.Model as TwitterSearch).TwitterResult.Count);
        }
    }
}
