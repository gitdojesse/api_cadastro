namespace ApiCadastro.Models
{
    public class Endereco
    {
        protected int Id;
        public string? Logradouro;
        public int Numero;
        public int Cep;
        public string? Bairro;
        public string? Cidade;
        public string? Estado;

        public int EnderecoId { get { return this.Id; } }
        public int SetId { set { this.Id = value; } }     
        
    }
}

