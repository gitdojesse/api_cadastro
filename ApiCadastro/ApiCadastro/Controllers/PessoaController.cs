using Microsoft.AspNetCore.Mvc;
using ApiCadastro.Models;
using ApiCadastro.Repositories;
using Newtonsoft.Json;

namespace ApiCadastro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PessoaController : ControllerBase
    {
        private readonly PessoaRepository _pessoaRepository;

        public PessoaController()
        {
            _pessoaRepository = new PessoaRepository();
        }

        [HttpGet]
        public ActionResult<List<Pessoa>> BuscarPessoas()
        {
            var pessoas = _pessoaRepository.BuscarPessoas;           
            var response = JsonConvert.SerializeObject(pessoas);
            return Ok(response);
        }

        [HttpGet("{cpf}")]
        public ActionResult BuscarPessoaPorCpf(long cpf)
        {
            Pessoa? pessoa = _pessoaRepository.BuscarPessoaPorCpf(cpf);
            if (pessoa != null)
            {
                var response = JsonConvert.SerializeObject(pessoa);
                return Ok(response);
            }
            return NotFound("Não foi encontrada nenhuma pessoa cadastrada com o CPF informado");
        }


        [HttpPost]
        public ActionResult<Pessoa> InserirPessoa([FromBody] Object post)
        {
            var succesInsert = _pessoaRepository.InserirPessoa(post);
            //return Ok("Pessoa cadastrad");
            if (!succesInsert)
            {
                return BadRequest("Já existe uma pessoa cadastrada com o CPF informado");
            }
            else
            {
                return Ok("Pessoa cadastrada com sucesso com o Id: " + succesInsert);
            }
        }

        /*
       [HttpDelete("{cpf}")]
       public ActionResult DeletarPessoa(Int64 cpf)
       {
           var isDeleted = _pessoaRepository.DeletarPessoa(cpf);
           if (isDeleted)
           {
               return NoContent();
           }
           return NotFound("Não foi encontrada nenhuma pessoa cadastrada com o CPF informado");
       }


       [HttpPut("{cpf}")]
       public ActionResult<Pessoa> AlterarPessoa(Int64 cpf, [FromBody] PessoaEnderecoTelefone pessoa)
       {
           var isChanged = _pessoaRepository.AlterarPessoa(cpf, pessoa);
           if (isChanged)
           {
               return NoContent();
           }
           return NotFound("Não foi encontrada nenhuma pessoa cadastrada com o CPF informado");
       }*/
    }
}