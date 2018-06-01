using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI;

namespace WebAPI.Controllers
{
    public class AdministratorsController : ApiController
    {
        private WebAPIEntities db = new WebAPIEntities();

        // GET: api/Administrators
        public IQueryable<Administrator> GetAdministrator()
        {
            return db.Administrator;
        }

        // GET: api/Administrators/5
        [ResponseType(typeof(Administrator))]
        public IHttpActionResult GetAdministrator(long id)
        {
            Administrator administrator = db.Administrator.Find(id);
            if (administrator == null)
            {
                return NotFound();
            }

            return Ok(administrator);
        }

        // PUT: api/Administrators/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAdministrator(long id, Administrator administrator)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != administrator.AdministratorID)
            {
                return BadRequest();
            }

            db.Entry(administrator).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdministratorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Administrators
        [ResponseType(typeof(Administrator))]
        public IHttpActionResult PostAdministrator(Administrator administrator)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Administrator.Add(administrator);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (AdministratorExists(administrator.AdministratorID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = administrator.AdministratorID }, administrator);
        }

        // DELETE: api/Administrators/5
        [ResponseType(typeof(Administrator))]
        public IHttpActionResult DeleteAdministrator(long id)
        {
            Administrator administrator = db.Administrator.Find(id);
            if (administrator == null)
            {
                return NotFound();
            }

            db.Administrator.Remove(administrator);
            db.SaveChanges();

            return Ok(administrator);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AdministratorExists(long id)
        {
            return db.Administrator.Count(e => e.AdministratorID == id) > 0;
        }
    }
}