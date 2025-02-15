using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;

namespace UniversiteRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EtudiantController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        // GET: api/<EtudiantController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<EtudiantController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        
        // Crée un nouvel étudiant sans parcours
        // POST api/<EtudiantController>
        [HttpPost]
        public async Task<EtudiantDto> PostAsync([FromBody] EtudiantDto etudiantDto)
        {
            // Converstion d'un EtudiantDto sans Id en Etudiant
            Etudiant etudiant = etudiantDto.ToEntity();
            // Création d'un étudiant
            CreateEtudiantUseCase uc=new CreateEtudiantUseCase(repositoryFactory);
            etudiant = await uc.ExecuteAsync(etudiant);
            // Conversion de l'étudiant avec son nouvel Id en EtudiantDto
            return new EtudiantDto().ToDto(etudiant);
        }

        // PUT api/<EtudiantController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<EtudiantController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
