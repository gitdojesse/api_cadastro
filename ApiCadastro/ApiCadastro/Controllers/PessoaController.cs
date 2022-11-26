using Microsoft.AspNetCore.Mvc;
using ApiCadastro.Models;
using ApiCadastro.Repositories;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;

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
            if (succesInsert)
            {
                return Ok("Cadastro realizado com sucesso");
            }
            else
            {
                return BadRequest("Falha ao cadastrar pessoa");
            }
        }

        [HttpPut("{cpf}")]
        public ActionResult<Pessoa> AlterarPessoa([FromBody] Object post)
        {
            var successUpdate = _pessoaRepository.AlterarPessoa(post);
            if (successUpdate)
            {
                return Ok("Alteração realizada com sucesso");
            }
            else           
            {
                return BadRequest("Falha ao alterar dados");
            }
        }

        [HttpDelete("{cpf}")]
        public ActionResult DeletarPessoa(long cpf)
        {
            var successUpdate = _pessoaRepository.DeletarPessoa(cpf);
            if (successUpdate)
            {
                return Ok("Exclusão realizada com sucesso");
            }
            else
            {
                return BadRequest("Falha ao excluir pessoa");
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