using ApiCadastro.Dao;
using ApiCadastro.Models;
using Microsoft.Extensions.Hosting;

namespace ApiCadastro.Repositories
{
    public class PessoaRepository
    {
        private readonly PessoaDao _pessoaDao;

        public PessoaRepository()
        {
            _pessoaDao = new PessoaDao();
        }

        public List<Pessoa> BuscarPessoas
        {
            get
            {
                return _pessoaDao.Listar();
            }
        }

        public Pessoa? BuscarPessoaPorCpf(long cpf)
        {
            {
                return _pessoaDao.Consulte(cpf);
            }
        }

        public bool InserirPessoa(Object post)
        {
            {               
                var succesInsert = _pessoaDao.Insira(post);
                return succesInsert;
            }
        }

        public bool AlterarPessoa(Object put)
        {
            {
                var successUpdate = _pessoaDao.Altere(put);
                return successUpdate;
            }
        }

        public bool DeletarPessoa(long cpf)
        {
            {
                var successDelete = _pessoaDao.Exclua(cpf);
                return successDelete;
            }
        }

    }
}
