namespace ApiCadastro.Models
{
    public class TipoTelefone
    {
        protected int Id;
        public string? Tipo;

        public int TipoTelefoneId { get { return this.Id; } }
        public int SetId { set { this.Id = value; } }
    }
}
