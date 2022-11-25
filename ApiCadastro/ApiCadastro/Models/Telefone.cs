namespace ApiCadastro.Models
{
    public class Telefone
    {
        protected int Id;
        public int Numero;
        public int Ddd;
        public TipoTelefone? Tipo;

        public int TelefoneId { get { return this.Id; } }
        public int SetId { set { this.Id = value; } }
        
    }
}
