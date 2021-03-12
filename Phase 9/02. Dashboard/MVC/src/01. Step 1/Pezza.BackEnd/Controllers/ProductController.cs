﻿namespace Pezza.BackEnd.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Pezza.Common;
    using Pezza.Common.DTO;
    using Pezza.Portal.Helpers;
    using Pezza.Portal.Models;

    public class ProductController : BaseController
    {
        private readonly ApiCallHelper<ProductDTO> apiCallHelper;

        public ProductController(IHttpClientFactory clientFactory)
            : base(clientFactory)
        {
            this.apiCallHelper = new ApiCallHelper<ProductDTO>(this.clientFactory);
            this.apiCallHelper.ControllerName = "Product";
        }

        public async Task<ActionResult> Index()
        {
            var json = JsonConvert.SerializeObject(new ProductDTO
            {
                PagingArgs = Common.Models.PagingArgs.NoPaging
            });
            var entities = await this.apiCallHelper.GetListAsync(json);
            for (var i = 0; i < entities.Count; i++)
            {
                entities[i].PictureUrl = $"{AppSettings.ApiUrl}Picture?file={entities[i].PictureUrl}&folder=Product";
            }
            return this.View(entities);
        }

        public async Task<ActionResult> Details(int id)
        {
            var entity = await this.apiCallHelper.GetAsync(id);
            return this.View(entity);
        }

        public ActionResult Create() => this.View(new ProductModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductModel product)
        {
            if (this.ModelState.IsValid)
            {
                if(string.IsNullOrEmpty(product.Description))
                {
                    product.Description = string.Empty;
                }

                if (product.Image?.Length > 0)
                {
                    using var ms = new MemoryStream();
                    product.Image.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    product.ImageData = $"data:{MimeTypeMap.GetMimeType(Path.GetExtension(product.Image.FileName))};base64,{Convert.ToBase64String(fileBytes)}";
                }

                var result = await this.apiCallHelper.Create(product);
                return this.RedirectToAction("Index");
            }
            else
            {
                return this.View(product);
            }
        }

        [Route("Product/Edit/{id?}")]
        public async Task<ActionResult> Edit(int id)
        {
            var entity = await this.apiCallHelper.GetAsync(id);
            return this.View(new ProductModel
            {
                Id = id,
                Name = entity.Name,
                Description = entity.Description,
                PictureUrl = $"{AppSettings.ApiUrl}Picture?file={entity.PictureUrl}&folder=Product",
                Price = entity.Price,
                Special = entity.Special,
                OfferEndDate = entity.OfferEndDate,
                OfferPrice = entity.OfferPrice,
                IsActive = entity.IsActive
            });
        }

        [HttpPost]
        [Route("Product/Edit/{id?}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, ProductModel product)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(product);
            }

            if (product.Image?.Length > 0)
            {
                using var ms = new MemoryStream();
                product.Image.CopyTo(ms);
                var fileBytes = ms.ToArray();
                product.ImageData = $"data:{MimeTypeMap.GetMimeType(Path.GetExtension(product.Image.FileName))};base64,{Convert.ToBase64String(fileBytes)}";
            }
            else
            {
                product.PictureUrl = null;
            }

            product.Id = id;
            var result = await this.apiCallHelper.Edit(product);
            return this.RedirectToAction("Index");
        }

        [HttpPost]
        [Route("Product/Delete/{id?}")]
        public async Task<JsonResult> Delete(int id)
        {
            if (id == 0)
            {
                return this.Json(false);
            }

            if (!this.ModelState.IsValid)
            {
                return this.Json(false);
            }

            var result = await this.apiCallHelper.Delete(id);
            return this.Json(result);
        }
    }
}
