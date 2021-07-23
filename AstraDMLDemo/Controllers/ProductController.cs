using AstraDMLDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AstraDMLDemo.Repositories;
using static AstraDMLDemo.Program;
using System.Diagnostics;
using RestSharp;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace AstraDMLDemo.Controllers
{
    /// <summary>
    /// Manages the business logic for the Product object
    /// </summary>
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private IProductRepository _productRepository;
        private readonly IConfiguration _config;
        private readonly ServiceResolver _serviceResolver;
        private long connectionDuration;

        /// <summary>
        /// ProductController constructor. Injects the logger, configuration, and serviceResolver objects.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        /// <param name="serviceResolver"></param>
        public ProductController(ILogger<ProductController> logger, IConfiguration config, ServiceResolver serviceResolver)
        {
            _logger = logger;
            _config = config;
            _serviceResolver = serviceResolver;
            ViewBag.ConnectionType = _connectionType;

            SetRepositoryHelper(ViewBag.ConnectionType);
        }

        /// <summary>
        /// Selects the appropriate repository based on repository parameter. Will also capture how the connection took
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        private bool SetRepositoryHelper(string repository)
        {
            // set the repository and capture the connection time
            var watch = new Stopwatch();

            // start the stopwatch
            watch.Start();

            try
            {
                _productRepository = _serviceResolver(repository);

                // check if the repository is available
                if (_productRepository.IsRepositoryAvailable())
                {
                    watch.Stop();
                    connectionDuration = watch.ElapsedMilliseconds;

                    return true;
                }
                else
                {
                    // set the respository type back to Memory
                    _productRepository = _serviceResolver(Enums.RepositoryType.Memory.ToString());
                    watch.Stop();
                    connectionDuration = watch.ElapsedMilliseconds;

                    return false;
                }

            }
            catch (Exception ex)
            {
                // most likely an error occured trying to connect
                // set the respository type back to Memory
                _productRepository = _serviceResolver(Enums.RepositoryType.Memory.ToString());
                watch.Stop();
                connectionDuration = watch.ElapsedMilliseconds;

                return false;
            }


        }

        // GET: ProductController
        /// <summary>
        /// Action for the Index or home page. The action will obtain a list of products and display via the Index view
        /// </summary>
        /// <param name="pg"></param>
        /// <returns></returns>
        public ActionResult Index(int pg=1)
        {
            // execute the GetAllProduct() command and capture the execution time
            var watch = new Stopwatch();
            watch.Start();
            IEnumerable<Product> products = _productRepository.GetAllProduct();
            watch.Stop();
            ViewBag.CommandDuration = watch.ElapsedMilliseconds;

            ViewBag.ConnectionType = _connectionType;
            ViewBag.RepositoryDefinition = _productRepository.RepositoryDefinition;
            ViewBag.ConnectionDuration = connectionDuration;

            const int pageSize = 7;
            if (pg < 1)
                pg = 1;

            int recsCount = products.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = products.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewBag.Pager = pager;

            // send the record counts to the Index view model
            ViewBag.TotalRecordCount = recsCount;
            ViewBag.PositiveCount = products.Where(p => p.field1.ToUpper().Equals("POSITIVE")).Count();
            ViewBag.NeutralCount = products.Where(p => p.field1.ToUpper().Equals("NEUTRAL")).Count();
            ViewBag.NegativeCount = products.Where(p => p.field1.ToUpper().Equals("NEGATIVE")).Count();

            return View(data);
        }

        /// <summary>
        /// Not implemented yet. Will delete the data in repository for a fresh start
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetRepositories()
        {
            // add code to reset repositories here. Should also create proper tables if they don't exist
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Action that will set the repository based on the repository parameter. The input field most likely 
        /// comes from the repository menu
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public IActionResult SetRepository(string repository)
        {
            _connectionType = repository;
            ViewBag.ConnectionType = _connectionType;

            // check if the repository is available. Dispaly product list if avaialable
            // or status page if not
            if (SetRepositoryHelper(repository))
            {
                ViewBag.Available = true;
                return RedirectToAction("Index", "Product");
            } else
            {
                ViewBag.Available = false;
                var rd = _repositoryDefinitions.FirstOrDefault(r => r.Name == repository);
                return View("RepositoryDefinitionDetail", rd);
            }
        }

        // GET: ProductController/Details/5
        /// <summary>
        /// Action that returns the details for a product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(Guid id)
        {
            try
            {
                return View(_productRepository.GetProduct(id));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: ProductController/Create
        /// <summary>
        /// Action to create a new product. The Faker library is used to populate the product
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            Product product = new Product();
            product.id = Guid.NewGuid();
            product.price = Faker.RandomNumber.Next(25);
            product.productname = Faker.Lorem.Words(1).First();
            product.description = Faker.Company.BS();
            product.created = DateTime.Now;
            product.field1 = Faker.Company.Name();
            product.field2 = Faker.Company.CatchPhrase();
            product.field3 = Faker.Internet.DomainName();
            product.field4 = Faker.Country.Name();
            product.field5 = Faker.Identification.UsPassportNumber();
            product.field6 = Faker.Company.Name();
            product.field7 = Faker.Address.City();
            product.field8 = Faker.Address.StreetAddress();
            product.field9 = Faker.Address.UsState();
            product.field10 = Faker.Address.ZipCode();

            return View(product);
        }

        // POST: ProductController/Create
        /// <summary>
        /// Action to create a new product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            // Get the sentiment of the product description
            SentimentResult sentiment = GetSentiment(product.description);
            product.field1 = sentiment.Sentiment;
            product.field2 = sentiment.Negativity;
            product.field3 = sentiment.Positivity;

            try
            {
                _productRepository.Add(product);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Action to obtain the sentiment for any text. Used to get the sentiment for product descriptions.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private SentimentResult GetSentiment(string text)
        {
            SentimentResult sentimentResult;

            // use AI to check the sentiment of the product description
            string key = _config.GetSection("Astra").GetSection("KenzyAiKey").Value;
            string encodedText = System.Web.HttpUtility.UrlEncode(text);
            string url = string.Format($"https://api.kenzyai.com/?key={key}&text={encodedText}");

            // Execute API Get 
            IRestResponse response = Utils.Api.Get(url, "");

            if (response.IsSuccessful)
            {
                // Get the results
                // Parse results
                JObject jObject = JObject.Parse(response.Content);
                JToken result = JToken.Parse(response.Content);
                sentimentResult = JsonConvert.DeserializeObject<SentimentResult>(result.ToString());
            }
            else
            {
                sentimentResult = new SentimentResult();
            }

            return sentimentResult;
        }

        // GET: ProductController/Edit/5
        /// <summary>
        /// Action to intiate a product edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(Guid id)
        {
            try
            {                
                return View(_productRepository.GetProduct(id));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ProductController/Edit/5
        /// <summary>
        /// Action to complete the editing of a product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Product product)
        {
            // Get the sentiment of the product description
            SentimentResult sentiment = GetSentiment(product.description);
            product.field1 = sentiment.Sentiment;
            product.field2 = sentiment.Negativity;
            product.field3 = sentiment.Positivity;

            try
            {
                _productRepository.Update(product);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductController/Delete/5
        /// <summary>
        /// Action to intiate the deletion of a product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(Guid id)
        {
            return View(_productRepository.GetProduct(id));
        }

        // POST: ProductController/Delete/5
        /// <summary>
        /// Action to complete the deletion of a product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid id, Product product)
        {
            try
            {
                // delete the Product
                _productRepository.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Action to check whether a connection is available for a defined repository
        /// </summary>
        /// <returns></returns>
        public ActionResult ConnectionStatus()
        {
            List<ConnectionStatusItem> connectionStatusItems = new List<ConnectionStatusItem>();
            bool available = false;

            foreach (RepositoryDefinition repositoryDefinition in _repositoryDefinitions)
            {
                try
                {
                    available = _serviceResolver(repositoryDefinition.Name).IsRepositoryAvailable();
                }
                catch 
                {
                    available = false;
                }
                connectionStatusItems.Add(new ConnectionStatusItem
                {
                    Name = repositoryDefinition.Name,
                    Available = available,
                    ServerType = repositoryDefinition.Server
                });
            }

            return View(connectionStatusItems);
        }
    }
}
