using ApiCadastro.Dao;
using ApiCadastro.Models;

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

        //public int InserirPessoa(Pessoa pessoa)
        //{
        //    {
        //        var pessoaId = _pessoaDao.InserirPessoa(pessoa);
        //        return pessoaId;
        //    }
        //}

        //public bool DeletarPessoa(Int64 cpf)
        //{
        //    {
        //        return _pessoaDao.DeletarPessoa(cpf);
        //    }
        //}

        //public bool AlterarPessoa(Int64 cpf, PessoaEnderecoTelefone pessoa)
        //{
        //    {
        //        return _pessoaDao.AlterarPessoa(cpf, pessoa);
        //    }
        //}
    }
}
