using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cludo_Assignment.Helpers
{
    public class SessionHelper
    {
        public const string INDEX_SESSION_NAME = "SESSIONINDEX";

        public static Directory GetIndex() => (Directory)HttpContextManager.Current.Session[INDEX_SESSION_NAME];

        public static void SetIndex(Directory index) => HttpContextManager.Current.Session[INDEX_SESSION_NAME] = index;
    }
}