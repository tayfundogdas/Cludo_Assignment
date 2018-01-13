using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Parameters;

namespace Business
{
    public class TwitterFlow
    {
        const string LUCENE_ID_NAME = "id";
        const string LUCENE_FIELD_NAME = "twitterText";
        private Lucene.Net.Store.Directory idx;

        public TwitterFlow(Lucene.Net.Store.Directory index)
        {
            this.idx = index;
        }

        public bool SaveSearchResultToFile(List<int> selectedIds)
        {
            var result = new List<KeyValuePair<int, string>>();

            //Setup searcher
            IndexSearcher searcher = new IndexSearcher(idx);

            //prepare search
            BooleanQuery query = new BooleanQuery();
            foreach (var id in selectedIds)
            {
                query.Add(new TermQuery(new Term(LUCENE_ID_NAME, id.ToString())), Occur.SHOULD);
            }

            //Do the search
            TopDocs hits = searcher.Search(query, int.MaxValue);

            if (hits != null)
            {
                ScoreDoc[] scoreDocs = hits.ScoreDocs;
                foreach (ScoreDoc scoreDoc in scoreDocs)
                {
                    Lucene.Net.Documents.Document doc = searcher.Doc(scoreDoc.Doc);
                    result.Add(new KeyValuePair<int, string>(Convert.ToInt32(doc.Get(LUCENE_ID_NAME)), doc.Get(LUCENE_FIELD_NAME)));
                }
            }

            //write result to file
            var dir = AppDomain.CurrentDomain.BaseDirectory + @"\output";
            var file = Path.Combine(dir, Guid.NewGuid() + ".txt");

            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);

            using (StreamWriter outputFile = File.CreateText(file))
            {
                foreach (var item in result)
                    outputFile.WriteLineAsync(item.Key + " - " + item.Value);
            }

            return true;
        }

        public List<KeyValuePair<int, string>> FilterSearchResult(string filterText)
        {
            var result = new List<KeyValuePair<int, string>>();

            //Setup searcher
            IndexSearcher searcher = new IndexSearcher(idx);

            //prepare search
            var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, LUCENE_FIELD_NAME, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
            var query = parser.Parse(filterText);

            //Query query = new TermQuery(new Term(LUCENE_FIELD_NAME, filterText));

            //Do the search
            TopDocs hits = searcher.Search(query, int.MaxValue);

            if (hits != null)
            {
                ScoreDoc[] scoreDocs = hits.ScoreDocs;
                foreach (ScoreDoc scoreDoc in scoreDocs)
                {
                    Lucene.Net.Documents.Document doc = searcher.Doc(scoreDoc.Doc);
                    result.Add(new KeyValuePair<int, string>(Convert.ToInt32(doc.Get(LUCENE_ID_NAME)), doc.Get(LUCENE_FIELD_NAME)));
                }
            }

            return result;
        }

        public void IndexSearchResult(List<KeyValuePair<int, string>> searchResult)
        {
            IndexWriter writer = new IndexWriter(idx, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30), IndexWriter.MaxFieldLength.UNLIMITED);

            foreach (var tweet in searchResult)
            {
                var doc = new Document();
                doc.Add(new Field(LUCENE_ID_NAME, tweet.Key.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field(LUCENE_FIELD_NAME, tweet.Value, Field.Store.YES, Field.Index.ANALYZED));
                writer.AddDocument(doc);
            }
            writer.Optimize();
            writer.Flush(true, true, true);
            writer.Commit();
        }

        public List<KeyValuePair<int, string>> SearchHashTagOnTwitter(string hashTag)
        {
            var result = new List<KeyValuePair<int, string>>();

            var searchParameter = new SearchTweetsParameters("#" + hashTag)
            {
                //GeoCode = new GeoCode(-122.398720, 37.781157, 1, DistanceMeasure.Miles),
                //Lang = LanguageFilter.English,
                //SearchType = SearchResultType.Popular,
                //MaximumNumberOfResults = 200,
                //Until = new DateTime(2015, 06, 02),
                //Since = new DateTime(2013, 12, 1),
                //SinceId = 399616835892781056,
                //MaxId = 405001488843284480,
                //Filters = TweetSearchFilters.Images
            };

            var tweets = Search.SearchTweets(searchParameter);

            if (tweets != null)
            {
                int i = 0;
                foreach (var tweet in tweets)
                {
                    result.Add(new KeyValuePair<int, string>(i, tweet.FullText));
                    ++i;
                }
            }

            return result;
        }

        /// <summary>
        /// It will always returns predefined data for test!
        /// </summary>
        /// <returns>Predefined data for test</returns>
        public List<KeyValuePair<int, string>> SearchHashTagOnTwitter()
        {
            var result = new List<KeyValuePair<int, string>>()
            {
                new KeyValuePair<int, string>(0,"#tayfun karali eğer bu ulkede adalet var diyorsanız bir insanı bu kadar küçük düşüren birine manevi tazminat davadı açılmalı" ),
                new KeyValuePair<int, string>(1,"#tayfun karali gibi kaba dayılar makam sahibi ise bu ülkede , vay halimize.."),
                new KeyValuePair<int, string>(3,"Hepsi toplanıp seni tokatlasaydı haktı! #tayfun karali"),
                new KeyValuePair<int, string>(4,"#tayfun karaali gibilere mevki verilmesin ona uzaklaştrma yetmez kovulsun hapis cezası tazminat cezası neyse almalı"),
                new KeyValuePair<int, string>(5,"Flaş gelişme! İBB #Zabıta Daire Başkanı #Tayfun Karali (KARAKTERSİZ) görevden uzaklaştırıldı http://hbr.tk/fYBYj0  @haberturk #Youtube"),
                new KeyValuePair<int, string>(6,"#Tayfun karalı  sıvas katlıamındakı sanıkların avukatı hıcte sasırmadım zalım yonetımden maZlum cokmayacaktı butun ınsanı duygularını yıtıren zavallı sadece zavallı bır hucrecıksın"),
                new KeyValuePair<int, string>(7,"Ya bi tek ben benzetiyor olamam heralde? #StrangerThings #steve #tayfun "),
            };

            return result;
        }
    }
}
