using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using SpotiFind.BusinessLogic;
using SpotiFind.Models;

namespace SpotiFind.Controllers
{
    public class LocationsController : ApiController
    {
        private SpotiFindContext db = new SpotiFindContext();
        private BusinessLogic.BusinessLogic businessLogic = new BusinessLogic.BusinessLogic();

        // GET: api/Locations
        public List<LocationDTO> GetLocations()
        {
            var locationsList = businessLogic.GetLocations();

            List<LocationDTO> locations = new List<LocationDTO>();
            
            foreach (Location location in locationsList)
            {
                locations.Add(new LocationDTO
                {
                    Id = location.Id,
                    Place = location.PlaceId,
                    PlaylistId = location.PlaylistId
                });
            }


            return locations;
        }

        // GET: api/Locations/5
        [ResponseType(typeof(LocationDetailDTO))]
        public async Task<IHttpActionResult> GetLocation(int id)
        {
            var l = businessLogic.GetLocationById(id);
            var p = businessLogic.GetPlaceById(id);

            if (l == null)
            {
                return NotFound();
            }

            var locationDetail = new LocationDetailDTO

            {
                Id = l.Id,
                PlaylistId = l.PlaylistId,

                PlaceName = p.Name,
                PlaceAddress = p.Address,
                
                PlaceLatitude = p.Latitude,
                PlaceLongitude = p.Longitude
            };

            return Ok(locationDetail);
        }

        // PUT: api/Locations/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLocation(int id, Location location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != location.Id)
            {
                return BadRequest();
            }

            db.Entry(location).State = System.Data.Entity.EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationExists(id))
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

        // POST: api/Locations
        [ResponseType(typeof(Location))]
        public async Task<IHttpActionResult> PostLocation(Location location)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Locations.Add(location);
            await db.SaveChangesAsync();

            var dto = new LocationDTO()
            {
                Id = location.Id,
                Place = location.PlaceId,
                PlaylistId = location.PlaylistId
            };

            return CreatedAtRoute("DefaultApi", new { id = location.Id }, dto);
        }

        // DELETE: api/Locations/5
        [ResponseType(typeof(Location))]
        public async Task<IHttpActionResult> DeleteLocation(int id)
        {
            Location location = await db.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            db.Locations.Remove(location);
            await db.SaveChangesAsync();

            return Ok(location);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LocationExists(int id)
        {
            return db.Locations.Count(e => e.Id == id) > 0;
        }
    }
}