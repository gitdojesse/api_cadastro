namespace ApiCadastro.Models
{
    public class Pessoa
    {
        protected int Id;
        public string? Nome;
        public long? Cpf;
        public Endereco? Endereco;
        public List<Telefone>? Telefones;

        public int PessoaId { get { return this.Id; } }
        public int SetId { set { this.Id = value; } }

    }
}
