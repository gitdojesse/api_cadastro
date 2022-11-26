
using ApiCadastro.Models;
//using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
//using System.Text.Json;

namespace ApiCadastro.Dao
{
    public class PessoaDao
    {
        string conexao = "Data Source=PC-WINDOWS\\SQLEXPRESS;Initial Catalog=dbcadastro;Integrated Security=True;MultipleActiveResultSets=true";

        public Pessoa? Consulte(long cpf)
        {
            using (SqlConnection conn = new(conexao))
            {
                conn.Open();
                using (SqlCommand cmd = new("SELECT * FROM pessoa WHERE cpf = @cpf", conn))
                {
                    cmd.Parameters.AddWithValue("cpf", cpf);
                    cmd.CommandType = System.Data.CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();

                            Pessoa pessoa = new();
                            Endereco? endereco = this.GetEndereco(reader.GetInt32("endereco"));
                            List<Telefone>? telefones = this.GetTelefoneByPessoa(reader.GetInt32("id"));

                            pessoa.SetId = reader.GetInt32("id");
                            pessoa.Nome = reader.GetString("nome");
                            pessoa.Cpf = reader.GetInt64("cpf");
                            pessoa.Endereco = endereco;
                            pessoa.Telefones = telefones;
                            return pessoa;
                        }
                        return null;
                    }
                }
            }
        }

        public bool Insira(Object post)
        {
            Pessoa? pessoaPost = JsonConvert.DeserializeObject<Pessoa>(post.ToString());
            Pessoa pessoaInsert = new Pessoa();
            Endereco enderecoInsert = new Endereco();
            Telefone? telefoneInsert = new Telefone();
            TipoTelefone tipoTelefoneInsert = new TipoTelefone();

            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();

                SqlCommand cmd = conn.CreateCommand();
                SqlTransaction transaction;

                transaction = conn.BeginTransaction();

                cmd.Connection = conn;
                cmd.Transaction = transaction;

                try
                {
                    if (pessoaPost.Cpf != null)
                    {
                        if (this.Consulte((long)pessoaPost.Cpf) != null)
                        {
                            throw new Exception("CPF já cadastrado");
                        }
                    }

                    cmd.CommandText = "INSERT INTO pessoa (nome, cpf, endereco) output INSERTED.ID VALUES (@nome, @cpf, @endereco)";
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("nome", pessoaPost.Nome);
                    cmd.Parameters.AddWithValue("cpf", pessoaPost.Cpf);

                    if (pessoaPost.Endereco != null)
                    {
                        Endereco? enderecoConsulta = this.GetEnderecoByCepAndNumero(pessoaPost.Endereco.Cep, pessoaPost.Endereco.Numero);

                        if (enderecoConsulta != null)
                        {
                            pessoaInsert.Endereco = enderecoConsulta;
                        }
                        else
                        {
                            enderecoInsert = this.InserirEndereco(pessoaPost.Endereco);
                            pessoaInsert.Endereco = enderecoInsert;
                        }
                        cmd.Parameters.AddWithValue("endereco", pessoaInsert.Endereco.EnderecoId);
                    }
                    else
                    {
                        throw new Exception("Endereço não informado");
                    }

                    int lastId = (int)cmd.ExecuteScalar();
                    pessoaInsert.SetId = lastId;
                    transaction.Commit();

                    if (pessoaPost.Telefones != null && pessoaPost.Telefones.GetType() == typeof(List<Telefone>))
                    {
                        foreach (Telefone telefonePost in pessoaPost.Telefones)
                        {
                            Telefone? telefoneConsulta = this.GetTelefone(telefonePost.Ddd, telefonePost.Numero);

                            if (telefoneConsulta != null)
                            {
                                this.InserirPessoaTelefone(pessoaInsert, telefoneConsulta);
                                continue;
                            }

                            if (telefonePost.Tipo != null && telefonePost.Tipo.Tipo != null)
                            {
                                TipoTelefone? tipoTelefoneConsulta = this.GetTelefoneTipo(telefonePost.Tipo.Tipo);

                                if (tipoTelefoneConsulta != null)
                                {
                                    telefonePost.Tipo = tipoTelefoneConsulta;
                                    telefoneInsert = this.InserirTelefone(telefonePost);
                                }
                                else
                                {
                                    tipoTelefoneInsert = this.InserirTipoTelefone(telefonePost.Tipo);
                                    telefonePost.Tipo = tipoTelefoneInsert;
                                    telefoneInsert = this.InserirTelefone(telefonePost);
                                }

                                if (telefoneInsert != null)
                                {
                                    this.InserirPessoaTelefone(pessoaInsert, telefoneInsert);
                                }
                            }
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                    Console.WriteLine("Message: {0}", ex.Message);

                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                        Console.WriteLine("Message: {0}", ex2.Message);
                    }
                    return false;
                }
            }
        }

        public bool Altere(Object put)
        {
            Pessoa? pessoaPut = JsonConvert.DeserializeObject<Pessoa>(put.ToString());
            Pessoa pessoaUpdate = new Pessoa();
            Endereco enderecoInsert = new Endereco();
            Telefone? telefoneInsert = new Telefone();
            TipoTelefone tipoTelefoneInsert = new TipoTelefone();

            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();

                SqlCommand cmd = conn.CreateCommand();
                SqlTransaction transaction;

                transaction = conn.BeginTransaction();

                cmd.Connection = conn;
                cmd.Transaction = transaction;

                try
                {
                    if (pessoaPut.Cpf != null)
                    {
                        Pessoa? pessoaConsulta = this.Consulte((long)pessoaPut.Cpf);

                        if (pessoaConsulta != null)
                        {
                            pessoaPut.SetId = pessoaConsulta.PessoaId;
                        }
                        else
                        {
                            throw new Exception("Pessoa não cadastrado");
                        }
                    }

                    cmd.CommandText = "UPDATE pessoa SET nome = @nome, cpf = @cpf, endereco = @endereco WHERE id = @id";
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("id", pessoaPut.PessoaId);
                    cmd.Parameters.AddWithValue("nome", pessoaPut.Nome);
                    cmd.Parameters.AddWithValue("cpf", pessoaPut.Cpf);

                    if (pessoaPut.Endereco != null)
                    {
                        Endereco? enderecoConsulta = this.GetEnderecoByCepAndNumero(pessoaPut.Endereco.Cep, pessoaPut.Endereco.Numero);

                        if (enderecoConsulta != null)
                        {
                            pessoaUpdate.Endereco = enderecoConsulta;
                        }
                        else
                        {
                            enderecoInsert = this.InserirEndereco(pessoaPut.Endereco);
                            pessoaUpdate.Endereco = enderecoInsert;
                        }
                        cmd.Parameters.AddWithValue("endereco", pessoaUpdate.Endereco.EnderecoId);
                    }
                    else
                    {
                        throw new Exception("Endereço não informado");
                    }

                    cmd.ExecuteReader();                    
                    transaction.Commit();

                    this.DeletePessoaTelefone(pessoaPut);
                    
                    if (pessoaPut.Telefones != null && pessoaPut.Telefones.GetType() == typeof(List<Telefone>))
                    {
                        foreach (Telefone telefonePost in pessoaPut.Telefones)
                        {
                            Telefone? telefoneConsulta = this.GetTelefone(telefonePost.Ddd, telefonePost.Numero);

                            if (telefoneConsulta != null)
                            {
                                this.InserirPessoaTelefone(pessoaPut, telefoneConsulta);
                                continue;
                            }

                            if (telefonePost.Tipo != null && telefonePost.Tipo.Tipo != null)
                            {
                                TipoTelefone? tipoTelefoneConsulta = this.GetTelefoneTipo(telefonePost.Tipo.Tipo);

                                if (tipoTelefoneConsulta != null)
                                {
                                    telefonePost.Tipo = tipoTelefoneConsulta;
                                    telefoneInsert = this.InserirTelefone(telefonePost);
                                }
                                else
                                {
                                    tipoTelefoneInsert = this.InserirTipoTelefone(telefonePost.Tipo);
                                    telefonePost.Tipo = tipoTelefoneInsert;
                                    telefoneInsert = this.InserirTelefone(telefonePost);
                                }

                                if (telefoneInsert != null)
                                {
                                    this.InserirPessoaTelefone(pessoaPut, telefoneInsert);
                                }
                            }
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                    Console.WriteLine("Message: {0}", ex.Message);

                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                        Console.WriteLine("Message: {0}", ex2.Message);
                    }
                    return false;
                }
            }
        }

        public bool Exclua (long cpf)
        {
            using (SqlConnection conn = new(conexao))
            {
                conn.Open();
                using (SqlCommand cmd = new("DELETE FROM pessoa WHERE cpf = @cpf", conn))
                {
                    cmd.Parameters.AddWithValue("cpf", cpf);
                    cmd.CommandType = System.Data.CommandType.Text;
                    int linhasAfetadas = cmd.ExecuteNonQuery();
                    
                    if (linhasAfetadas > 0)
                    {
                        return true;
                    }
                    return false;
                    
                }
            }
        }
        
        private Endereco? GetEndereco(int idEndereco)
        {          
            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();                
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM endereco WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", idEndereco);
                    cmd.CommandType = System.Data.CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        
                        if (reader.HasRows)
                        {

                            reader.Read();
                            Endereco endereco = new Endereco();
                            endereco.SetId = reader.GetInt32("id");
                            endereco.Logradouro = reader.GetString("logradouro");
                            endereco.Numero = reader.GetInt32("numero");
                            endereco.Cep = reader.GetInt32("cep");
                            endereco.Bairro = reader.GetString("bairro");
                            endereco.Cidade = reader.GetString("cidade");
                            endereco.Estado = reader.GetString("estado");
                            return endereco;
                        } 
                        return null;                    
                    }
                }                
            }
        }

        private Endereco? GetEnderecoByCepAndNumero(int cep, int numero)
        {
            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM endereco WHERE cep = @cep AND numero = @numero", conn))
                {
                    cmd.Parameters.AddWithValue("cep", cep);
                    cmd.Parameters.AddWithValue("numero", numero);
                    cmd.CommandType = System.Data.CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        if (reader.HasRows)
                        {
                            reader.Read();
                            Endereco endereco = new Endereco();
                            endereco.SetId = reader.GetInt32("id");
                            endereco.Logradouro = reader.GetString("logradouro");
                            endereco.Numero = reader.GetInt32("numero");
                            endereco.Cep = reader.GetInt32("cep");
                            endereco.Bairro = reader.GetString("bairro");
                            endereco.Cidade = reader.GetString("cidade");
                            endereco.Estado = reader.GetString("estado");
                            return endereco;
                        }
                        return null;
                    }
                }

            }
        }

        private List<Telefone>? GetTelefoneByPessoa(int idPessoa)
        {           
            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM pessoa_telefone WHERE id_pessoa = @idPessoa", conn))
                {
                    cmd.Parameters.AddWithValue("idPessoa", idPessoa);
                    cmd.CommandType = System.Data.CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            List<Telefone> telefones = new List<Telefone>();

                            while (reader.Read())
                            {
                                Telefone? telefone = this.GetTelefone(reader.GetInt32("id_telefone"));

                                if (telefone != null)
                                {
                                    telefones.Add(telefone);
                                }
                            }
                            return telefones;
                        }
                        return null;
                    }
                }
            }            
        }

        private Telefone? GetTelefone(int idTelefone)
        {
            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM telefone WHERE id = @idTelefone", conn))
                {
                    cmd.Parameters.AddWithValue("idTelefone", idTelefone);
                    cmd.CommandType = System.Data.CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        if (reader.HasRows)
                        {
                            reader.Read();                            
                            TipoTelefone? tipo  = this.GetTelefoneTipo(reader.GetInt32("tipo"));
                            Telefone telefone   = new Telefone();

                            telefone.SetId  = reader.GetInt32("id");
                            telefone.Numero = reader.GetInt32("numero");
                            telefone.Ddd    = reader.GetInt32("ddd");
                            telefone.Tipo   = tipo;

                            return telefone;
                        }
                        return null;
                    }
                }
            }
        }

        private Telefone? GetTelefone(int ddd, int telefone)
        {
            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM telefone WHERE ddd = @ddd AND numero = @telefone", conn))
                {
                    cmd.Parameters.AddWithValue("ddd", ddd);
                    cmd.Parameters.AddWithValue("telefone", telefone);
                    cmd.CommandType = System.Data.CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            TipoTelefone? tipo = this.GetTelefoneTipo(reader.GetInt32("tipo"));
                            Telefone objTelefone = new Telefone();

                            objTelefone.SetId = reader.GetInt32("id");
                            objTelefone.Numero = reader.GetInt32("numero");
                            objTelefone.Ddd = reader.GetInt32("ddd");
                            objTelefone.Tipo = tipo;

                            return objTelefone;
                        }
                        conn.Close();
                        return null;
                    }
                }
            }
        }

        private TipoTelefone? GetTelefoneTipo(int idTipo)
        {
            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM telefone_tipo WHERE id = @idTipo", conn))
                {
                    cmd.Parameters.AddWithValue("idTipo", idTipo);
                    cmd.CommandType = System.Data.CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        if (reader.HasRows)
                        {
                            reader.Read();
                            TipoTelefone tipoTelefone = new TipoTelefone();
                            
                            tipoTelefone.SetId  = reader.GetInt32("id");
                            tipoTelefone.Tipo   = reader.GetString("tipo");
                            
                            return tipoTelefone;
                        }
                        return null;
                    }
                }

            }
        }

        private TipoTelefone? GetTelefoneTipo(string? tipo)
        {
            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM telefone_tipo WHERE tipo = @tipo", conn))
                {
                    cmd.Parameters.AddWithValue("tipo", tipo);
                    cmd.CommandType = System.Data.CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        if (reader.HasRows)
                        {
                            reader.Read();
                            TipoTelefone tipoTelefone = new TipoTelefone();

                            tipoTelefone.SetId = reader.GetInt32("id");
                            tipoTelefone.Tipo = reader.GetString("tipo");

                            return tipoTelefone;
                        }
                        return null;
                    }
                }
            }
        }

        private Endereco InserirEndereco(Endereco endereco)
        {
            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "INSERT INTO endereco (logradouro, numero, cep, bairro, cidade, estado) output INSERTED.ID VALUES (@logradouro, @numero, @cep, @bairro, @cidade, @estado)";
                cmd.Parameters.AddWithValue("logradouro", endereco.Logradouro);
                cmd.Parameters.AddWithValue("numero", endereco.Numero);
                cmd.Parameters.AddWithValue("cep", endereco.Cep);
                cmd.Parameters.AddWithValue("bairro", endereco.Bairro);
                cmd.Parameters.AddWithValue("cidade", endereco.Cidade);
                cmd.Parameters.AddWithValue("estado", endereco.Estado);
                
                int lastId = (int) cmd.ExecuteScalar();

                endereco.SetId = lastId;
                return endereco;
            }
        }

        private TipoTelefone InserirTipoTelefone(TipoTelefone tipoTelefone)
        {            
            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "INSERT INTO telefone_tipo (tipo) output INSERTED.ID VALUES (@tipo)";
                cmd.Parameters.AddWithValue("tipo", tipoTelefone.Tipo);
                
                int lastId = (int) cmd.ExecuteScalar();
                conn.Open();
                tipoTelefone.SetId = lastId;
                return tipoTelefone;
            }            
        }

        private Telefone? InserirTelefone(Telefone telefone)
        {
            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "INSERT INTO telefone (numero, ddd, tipo) output INSERTED.ID VALUES (@numero, @ddd, @tipo)";
                cmd.Parameters.AddWithValue("numero", telefone.Numero);
                cmd.Parameters.AddWithValue("ddd", telefone.Ddd);
                
                if (telefone.Tipo != null)
                {
                    cmd.Parameters.AddWithValue("tipo", telefone.Tipo.TipoTelefoneId);
                    int lastId = (int) cmd.ExecuteScalar();
                    conn.Close();
                    telefone.SetId = lastId;
                    return telefone;
                }
                else
                {
                    conn.Close();
                    return null;
                }
            }            
        }

        private void InserirPessoaTelefone(Pessoa pessoa, Telefone telefone)
        {
            using (SqlConnection conn = new SqlConnection(conexao))
            {                
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "INSERT INTO pessoa_telefone (id_pessoa, id_telefone) output INSERTED.* VALUES (@id_pessoa, @id_telefone)";
                cmd.Parameters.AddWithValue("id_pessoa", pessoa.PessoaId);
                cmd.Parameters.AddWithValue("id_telefone", telefone.TelefoneId);
                cmd.ExecuteReader();

                conn.Close();
            }
        }

        private void DeletePessoaTelefone(Pessoa pessoa)
        {
            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "DELETE FROM pessoa_telefone WHERE id_pessoa = @id_pessoa";
                cmd.Parameters.AddWithValue("id_pessoa", pessoa.PessoaId);
                cmd.ExecuteReader();

                conn.Close();
            }
        }

        //public List<Pessoa> Listar()
        //{
        //    List<Pessoa> pessoas = new List<Pessoa>();

        //    using (SqlConnection conn = new SqlConnection(conexao))
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = new SqlCommand("SELECT * FROM pessoa", conn))
        //        {
        //            cmd.CommandType = System.Data.CommandType.Text;
        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                if (reader.HasRows)
        //                {
        //                    while (reader.Read())
        //                    {
        //                        Pessoa pessoa = new Pessoa();
        //                        Endereco? endereco = this.GetEndereco(reader.GetInt32("endereco"));
        //                        List<Telefone>? telefones = this.GetTelefoneByPessoa(reader.GetInt32("id"));

        //                        pessoa.SetId = reader.GetInt32("id");
        //                        pessoa.Nome = reader.GetString("nome");
        //                        pessoa.Cpf = reader.GetInt64("cpf");
        //                        pessoa.Endereco = endereco;
        //                        pessoa.Telefones = telefones;

        //                        pessoas.Add(pessoa);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return pessoas;
        //}        
    }
}

