
using ApiCadastro.Models;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;

namespace ApiCadastro.Dao
{
    public class PessoaDao
    {
        string conexao = "Data Source=PC-WINDOWS\\SQLEXPRESS;Initial Catalog=dbcadastro;Integrated Security=True";

        public List<Pessoa> Listar()
        {
            List<Pessoa> pessoas = new List<Pessoa>();

            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM pessoa", conn))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Pessoa pessoa            = new Pessoa();
                                Endereco? endereco       = this.GetEndereco(reader.GetInt32("endereco"));
                                List<Telefone>? telefones = this.GetTelefoneByPessoa(reader.GetInt32("id"));
                                
                                pessoa.SetId     = reader.GetInt32("id");
                                pessoa.Nome      = reader.GetString("nome");
                                pessoa.Cpf       = reader.GetInt64("cpf");
                                pessoa.Endereco  = endereco;
                                pessoa.Telefones = telefones;

                                pessoas.Add(pessoa);
                            }
                        }
                    }
                }
            }

            return pessoas;
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
                        if ( this.Consulte((long)pessoaPost.Cpf) != null ){
                            throw new Exception("CPF já cadastrado");
                        }
                    }

                    cmd.CommandText = "INSERT INTO pessoa (Nome, Cpf, Endereco) output INSERTED.ID VALUES (@NOME, @CPFINSERT, @ENDERECOID)";
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("NOME", pessoaPost.Nome);
                    cmd.Parameters.AddWithValue("CPFINSERT", pessoaPost.Cpf);

                    if (pessoaPost.Endereco != null)
                    {
                        Endereco? enderecoConsulta = this.GetEnderecoByCepAndNumero(pessoaPost.Endereco.Cep, pessoaPost.Endereco.Numero);

                        if ( enderecoConsulta != null )                                           
                        {
                            pessoaInsert.Endereco = enderecoConsulta;
                        }
                        else
                        {
                            enderecoInsert = this.InserirEndereco(pessoaPost.Endereco);
                            pessoaInsert.Endereco = enderecoInsert;
                        }
                        cmd.Parameters.AddWithValue("ENDERECOID", pessoaInsert.Endereco.EnderecoId);
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
                                Console.WriteLine("linha 369: " + pessoaInsert.PessoaId +"-"+ telefoneConsulta.TelefoneId);
                                this.InserirPessoaTelefone(pessoaInsert, telefoneConsulta);
                                continue;
                            }
                            else
                            {
                                Console.WriteLine(telefonePost.Numero + " telefoneConsulta não existe");
                                if (telefonePost.Tipo != null && telefonePost.Tipo.Tipo != null)
                                {
                                    TipoTelefone? tipoTelefoneConsulta = this.GetTelefoneTipo(telefonePost.Tipo.Tipo);

                                    if (tipoTelefoneConsulta != null)
                                    {
                                        Console.WriteLine("tipoTelefoneConsulta tem");
                                        telefonePost.Tipo = tipoTelefoneConsulta;
                                        telefoneInsert = this.InserirTelefone(telefonePost);
                                        //this.InserirPessoaTelefone(pessoaInsert, telefoneInsert);
                                        //continue;
                                    }
                                    else
                                    {
                                        tipoTelefoneInsert = this.InserirTipoTelefone(telefonePost.Tipo);                                                                              
                                        telefonePost.Tipo = tipoTelefoneInsert;
                                        telefoneInsert = this.InserirTelefone(telefonePost);

                                        //if (telefoneInsert != null)
                                        //{
                                        //    this.InserirPessoaTelefone(pessoaInsert, telefoneInsert);
                                        //}                                        
                                    }

                                    if (telefoneInsert != null)
                                    {
                                        Console.WriteLine("linha 405: " + pessoaInsert.PessoaId + "-" + telefoneInsert.TelefoneId);
                                        this.InserirPessoaTelefone(pessoaInsert, telefoneInsert);
                                    }
                                }
                            }
                        }

                        //throw new Exception("Testes");
                    }

                    //transaction.Commit();
                    return true;

                    /*
                    cmd.CommandText = "SELECT * FROM tb_tipos_telefone WHERE Tipo = @TIPOTELEFONE";
                    cmd.Parameters.AddWithValue("TIPOTELEFONE", pessoa.TipoTelefone);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            tipoTelefoneId = (int)reader["Id"];
                        }
                        reader.Close();
                        reader.Dispose();
                    }

                    if (tipoTelefoneId == -1)
                    {
                        cmd.CommandText = "INSERT INTO tb_tipos_telefone (Tipo) output INSERTED.ID VALUES (@TIPOTELEFONEINSERT)";
                        cmd.Parameters.AddWithValue("TIPOTELEFONEINSERT", pessoa.TipoTelefone);
                        tipoTelefoneId = (int)cmd.ExecuteScalar();
                    }

                    cmd.CommandText = "SELECT * FROM tb_telefones WHERE Numero = @NUMEROTELEFONE AND Ddd = @DDD";
                    cmd.Parameters.AddWithValue("NUMEROTELEFONE", pessoa.NumeroTelefone);
                    cmd.Parameters.AddWithValue("DDD", pessoa.Ddd);
                    SqlDataReader readerTelefone = cmd.ExecuteReader();

                    if (readerTelefone != null)
                    {
                        while (readerTelefone.Read())
                        {
                            telefoneId = (int)readerTelefone["Id"];
                        }
                        readerTelefone.Close();
                        readerTelefone.Dispose();
                    }

                    if (telefoneId == -1)
                    {
                        cmd.CommandText = "INSERT INTO tb_telefones (Numero, Ddd, TipoTelefoneId) output INSERTED.ID VALUES (@NUMEROTELEFONEINSERT, @DDDINSERT, @TIPOTELEFONEID)";
                        cmd.Parameters.AddWithValue("NUMEROTELEFONEINSERT", pessoa.NumeroTelefone);
                        cmd.Parameters.AddWithValue("DDDINSERT", pessoa.Ddd);
                        cmd.Parameters.AddWithValue("TIPOTELEFONEID", tipoTelefoneId);
                        telefoneId = (int)cmd.ExecuteScalar();
                    }

                    cmd.CommandText = "INSERT INTO tb_enderecos (Logradouro, Numero, Cep, Bairro, Cidade, Estado) output INSERTED.ID VALUES (@LOGRADOURO, @NUMERO, @CEP, @BAIRRO, @CIDADE, @ESTADO)";
                    cmd.Parameters.AddWithValue("LOGRADOURO", pessoa.Logradouro);
                    cmd.Parameters.AddWithValue("NUMERO", pessoa.Numero);
                    cmd.Parameters.AddWithValue("CEP", pessoa.Cep);
                    cmd.Parameters.AddWithValue("BAIRRO", pessoa.Bairro);
                    cmd.Parameters.AddWithValue("CIDADE", pessoa.Cidade);
                    cmd.Parameters.AddWithValue("ESTADO", pessoa.Estado);
                    enderecoId = (int)cmd.ExecuteScalar();

                    cmd.CommandText = "INSERT INTO tb_pessoas (Nome, Cpf, EnderecoId) output INSERTED.ID VALUES (@NOME, @CPFINSERT, @ENDERECOID)";
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("NOME", pessoa.Nome);
                    cmd.Parameters.AddWithValue("CPFINSERT", pessoa.Cpf);
                    cmd.Parameters.AddWithValue("ENDERECOID", enderecoId);
                    pessoaId = (int)cmd.ExecuteScalar();

                    cmd.CommandText = "INSERT INTO tb_pessoa_telefone (PessoaId, TelefoneId) VALUES (@PESSOAID, @TELEFONEID)";
                    cmd.Parameters.AddWithValue("PESSOAID", pessoaId);
                    cmd.Parameters.AddWithValue("TELEFONEID", telefoneId);
                    cmd.ExecuteScalar();

                    transaction.Commit();
                    return pessoaId; */
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

        /*
        public int InserirPessoa(PessoaEnderecoTelefone pessoa)
        {
            int tipoTelefoneId = -1;
            int pessoaId = -1;
            int telefoneId = -1;
            int enderecoId = -1;
            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();

                SqlCommand command = conn.CreateCommand();
                SqlTransaction transaction;

                transaction = conn.BeginTransaction();

                command.Connection = conn;
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "SELECT * FROM tb_pessoas WHERE Cpf = @CPF";
                    command.Parameters.AddWithValue("CPF", pessoa.Cpf);
                    SqlDataReader readerPessoa = command.ExecuteReader();

                    if (readerPessoa != null)
                    {
                        while (readerPessoa.Read())
                        {
                            pessoaId = (int)readerPessoa["Id"];
                        }
                        readerPessoa.Close();
                        readerPessoa.Dispose();
                    }

                    if (pessoaId != -1)
                    {
                        pessoaId = -1;
                        throw new Exception("CPF já cadastrado");
                    }

                    command.CommandText = "SELECT * FROM tb_tipos_telefone WHERE Tipo = @TIPOTELEFONE";
                    command.Parameters.AddWithValue("TIPOTELEFONE", pessoa.TipoTelefone);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            tipoTelefoneId = (int)reader["Id"];
                        }
                        reader.Close();
                        reader.Dispose();
                    }

                    if (tipoTelefoneId == -1)
                    {
                        command.CommandText = "INSERT INTO tb_tipos_telefone (Tipo) output INSERTED.ID VALUES (@TIPOTELEFONEINSERT)";
                        command.Parameters.AddWithValue("TIPOTELEFONEINSERT", pessoa.TipoTelefone);
                        tipoTelefoneId = (int)command.ExecuteScalar();
                    }

                    command.CommandText = "SELECT * FROM tb_telefones WHERE Numero = @NUMEROTELEFONE AND Ddd = @DDD";
                    command.Parameters.AddWithValue("NUMEROTELEFONE", pessoa.NumeroTelefone);
                    command.Parameters.AddWithValue("DDD", pessoa.Ddd);
                    SqlDataReader readerTelefone = command.ExecuteReader();

                    if (readerTelefone != null)
                    {
                        while (readerTelefone.Read())
                        {
                            telefoneId = (int)readerTelefone["Id"];
                        }
                        readerTelefone.Close();
                        readerTelefone.Dispose();
                    }

                    if (telefoneId == -1)
                    {
                        command.CommandText = "INSERT INTO tb_telefones (Numero, Ddd, TipoTelefoneId) output INSERTED.ID VALUES (@NUMEROTELEFONEINSERT, @DDDINSERT, @TIPOTELEFONEID)";
                        command.Parameters.AddWithValue("NUMEROTELEFONEINSERT", pessoa.NumeroTelefone);
                        command.Parameters.AddWithValue("DDDINSERT", pessoa.Ddd);
                        command.Parameters.AddWithValue("TIPOTELEFONEID", tipoTelefoneId);
                        telefoneId = (int)command.ExecuteScalar();
                    }

                    command.CommandText = "INSERT INTO tb_enderecos (Logradouro, Numero, Cep, Bairro, Cidade, Estado) output INSERTED.ID VALUES (@LOGRADOURO, @NUMERO, @CEP, @BAIRRO, @CIDADE, @ESTADO)";
                    command.Parameters.AddWithValue("LOGRADOURO", pessoa.Logradouro);
                    command.Parameters.AddWithValue("NUMERO", pessoa.Numero);
                    command.Parameters.AddWithValue("CEP", pessoa.Cep);
                    command.Parameters.AddWithValue("BAIRRO", pessoa.Bairro);
                    command.Parameters.AddWithValue("CIDADE", pessoa.Cidade);
                    command.Parameters.AddWithValue("ESTADO", pessoa.Estado);
                    enderecoId = (int)command.ExecuteScalar();

                    command.CommandText = "INSERT INTO tb_pessoas (Nome, Cpf, EnderecoId) output INSERTED.ID VALUES (@NOME, @CPFINSERT, @ENDERECOID)";
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("NOME", pessoa.Nome);
                    command.Parameters.AddWithValue("CPFINSERT", pessoa.Cpf);
                    command.Parameters.AddWithValue("ENDERECOID", enderecoId);
                    pessoaId = (int)command.ExecuteScalar();

                    command.CommandText = "INSERT INTO tb_pessoa_telefone (PessoaId, TelefoneId) VALUES (@PESSOAID, @TELEFONEID)";
                    command.Parameters.AddWithValue("PESSOAID", pessoaId);
                    command.Parameters.AddWithValue("TELEFONEID", telefoneId);
                    command.ExecuteScalar();

                    transaction.Commit();
                    return pessoaId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);

                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                        Console.WriteLine("  Message: {0}", ex2.Message);
                    }
                    return pessoaId;
                }


                //using (SqlCommand cmd = new SqlCommand("SELECT * FROM tb_tipos_telefone WHERE Tipo = @TIPOTELEFONE ", conn))
                //{
                //cmd.CommandType = System.Data.CommandType.Text;
                //cmd.Parameters.AddWithValue("TIPOTELEFONE", pessoa.TipoTelefone);
                //SqlDataReader reader = cmd.ExecuteReader();

                //    if (reader != null)
                //    {
                //        while (reader.Read())
                //        {
                //            tipoTelefoneId = (int)reader["Id"];
                //        }
                //        reader.Close();
                //        reader.Dispose();
                //    }

                //    if (tipoTelefoneId == -1)
                //    {
                //        using (SqlCommand tipoTelefoneCmd = new SqlCommand("INSERT INTO tb_tipos_telefone (Tipo) output INSERTED.ID VALUES (@TIPOTELEFONE)", conn))
                //        {
                //            tipoTelefoneCmd.CommandType = System.Data.CommandType.Text;
                //            tipoTelefoneCmd.Parameters.AddWithValue("TIPOTELEFONE", pessoa.TipoTelefone);
                //            tipoTelefoneId = (int)tipoTelefoneCmd.ExecuteScalar();
                //        }
                //    }
                //}

                //using (SqlCommand cmd = new SqlCommand("INSERT INTO tb_telefones (Numero, Ddd, TipoTelefoneId) output INSERTED.ID VALUES (@NUMEROTELEFONE, @DDD, @TIPOTELEFONEID)", conn))
                //{
                //    cmd.CommandType = System.Data.CommandType.Text;
                //    cmd.Parameters.AddWithValue("NUMEROTELEFONE", pessoa.NumeroTelefone);
                //    cmd.Parameters.AddWithValue("DDD", pessoa.Ddd);
                //    cmd.Parameters.AddWithValue("TIPOTELEFONEID", tipoTelefoneId);
                //    telefoneId = (int)cmd.ExecuteScalar();
                //}

                //using (SqlCommand cmd = new SqlCommand("INSERT INTO tb_enderecos (Logradouro, Numero, Cep, Bairro, Cidade, Estado) output INSERTED.ID VALUES (@LOGRADOURO, @NUMERO, @CEP, @BAIRRO, @CIDADE, @ESTADO)", conn))
                //{
                //    cmd.CommandType = System.Data.CommandType.Text;
                //    cmd.Parameters.AddWithValue("LOGRADOURO", pessoa.Logradouro);
                //    cmd.Parameters.AddWithValue("NUMERO", pessoa.Numero);
                //    cmd.Parameters.AddWithValue("CEP", pessoa.Cep);
                //    cmd.Parameters.AddWithValue("BAIRRO", pessoa.Bairro);
                //    cmd.Parameters.AddWithValue("CIDADE", pessoa.Cidade);
                //    cmd.Parameters.AddWithValue("ESTADO", pessoa.Estado);
                //    enderecoId = (int)cmd.ExecuteScalar();
                //}

                //using (SqlCommand cmd = new SqlCommand("INSERT INTO tb_pessoas (Nome, Cpf, EnderecoId) output INSERTED.ID VALUES (@NOME, @CPF, @ENDERECOID)", conn))
                //{
                //    cmd.CommandType = System.Data.CommandType.Text;
                //    cmd.Parameters.AddWithValue("NOME", pessoa.Nome);
                //    cmd.Parameters.AddWithValue("CPF", pessoa.Cpf);
                //    cmd.Parameters.AddWithValue("ENDERECOID", enderecoId);
                //    pessoaId = (int)cmd.ExecuteScalar();
                //}

                //using (SqlCommand cmd = new SqlCommand("INSERT INTO tb_pessoa_telefone (PessoaId, TelefoneId) VALUES (@PESSOAID, @TELEFONEID);", conn))
                //{
                //    cmd.CommandType = System.Data.CommandType.Text;
                //    cmd.Parameters.AddWithValue("PESSOAID", pessoaId);
                //    cmd.Parameters.AddWithValue("TELEFONEID", telefoneId);
                //    cmd.ExecuteScalar();
                //}
            }
        }

        public bool DeletarPessoa(Int64 cpf)
        {
            var pessoa = new Pessoa();
            int rowsAffected = -1;
            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM tb_pessoas WHERE Cpf = " + cpf, conn))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                pessoa.Id = (int?)reader["Id"];
                            }
                            reader.Close();
                            reader.Dispose();
                        }

                        if (pessoa.Id == null)
                        {
                            return false;
                        }

                        using (SqlCommand deleteCmd = new SqlCommand("DELETE FROM tb_pessoas WHERE Cpf = @CPFDELETE", conn))
                        {
                            deleteCmd.CommandType = System.Data.CommandType.Text;
                            deleteCmd.Parameters.AddWithValue("CPFDELETE", cpf);
                            rowsAffected = deleteCmd.ExecuteNonQuery();
                        }

                        if (rowsAffected > 0)
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
        }

        public bool AlterarPessoa(Int64 cpf, PessoaEnderecoTelefone pessoa)
        {
            int tipoTelefoneId = -1;
            int pessoaId = -1;
            int telefoneId = -1;
            int enderecoId = -1;
            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();

                SqlCommand command = conn.CreateCommand();
                SqlTransaction transaction;

                transaction = conn.BeginTransaction();

                command.Connection = conn;
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "SELECT * FROM tb_pessoas WHERE Cpf = @CPF";
                    command.Parameters.AddWithValue("CPF", cpf);
                    SqlDataReader readerPessoa = command.ExecuteReader();

                    if (readerPessoa != null)
                    {
                        while (readerPessoa.Read())
                        {
                            pessoaId = (int)readerPessoa["Id"];
                            enderecoId = (int)readerPessoa["EnderecoId"];
                        }
                        readerPessoa.Close();
                        readerPessoa.Dispose();
                    }

                    if (pessoaId == -1)
                    {
                        throw new Exception("CPF não cadastrado");
                    }

                    command.CommandText = "SELECT * FROM tb_tipos_telefone WHERE Tipo = @TIPOTELEFONE";
                    command.Parameters.AddWithValue("TIPOTELEFONE", pessoa.TipoTelefone);
                    SqlDataReader readerTiposTelefone = command.ExecuteReader();

                    if (readerTiposTelefone != null)
                    {
                        while (readerTiposTelefone.Read())
                        {
                            tipoTelefoneId = (int)readerTiposTelefone["Id"];
                        }
                        readerTiposTelefone.Close();
                        readerTiposTelefone.Dispose();
                    }

                    if (tipoTelefoneId == -1)
                    {
                        command.CommandText = "INSERT INTO tb_tipos_telefone (Tipo) output INSERTED.ID VALUES (@TIPOTELEFONEINSERT)";
                        command.Parameters.AddWithValue("TIPOTELEFONEINSERT", pessoa.TipoTelefone);
                        tipoTelefoneId = (int)command.ExecuteScalar();
                    }

                    command.CommandText = "SELECT * FROM tb_pessoa_telefone WHERE PessoaId = @PESSOAID";
                    command.Parameters.AddWithValue("PESSOAID", pessoaId);
                    SqlDataReader readerPessoaTelefone = command.ExecuteReader();

                    if (readerPessoaTelefone != null)
                    {
                        while (readerPessoaTelefone.Read())
                        {
                            telefoneId = (int)readerPessoaTelefone["TelefoneId"];
                        }
                        readerPessoaTelefone.Close();
                        readerPessoaTelefone.Dispose();
                    }

                    command.CommandText = "UPDATE tb_telefones SET Numero = @NUMEROTELEFONE, Ddd = @DDD WHERE Id = @TELEFONEID";
                    command.Parameters.AddWithValue("NUMEROTELEFONE", pessoa.NumeroTelefone);
                    command.Parameters.AddWithValue("DDD", pessoa.Ddd);
                    command.Parameters.AddWithValue("TELEFONEID", telefoneId);
                    command.ExecuteScalar();

                    command.CommandText = "UPDATE tb_enderecos SET Logradouro = @LOGRADOURO, Numero = @NUMERO, Cep = @CEP, Bairro = @BAIRRO, Cidade = @CIDADE, Estado = @ESTADO WHERE Id = @ENDERECOID";
                    command.Parameters.AddWithValue("LOGRADOURO", pessoa.Logradouro);
                    command.Parameters.AddWithValue("NUMERO", pessoa.Numero);
                    command.Parameters.AddWithValue("CEP", pessoa.Cep);
                    command.Parameters.AddWithValue("BAIRRO", pessoa.Bairro);
                    command.Parameters.AddWithValue("CIDADE", pessoa.Cidade);
                    command.Parameters.AddWithValue("ESTADO", pessoa.Estado);
                    command.Parameters.AddWithValue("ENDERECOID", enderecoId);
                    command.ExecuteScalar();

                    command.CommandText = "UPDATE tb_pessoas SET Nome = @NOME, Cpf = @CPFUPDATE WHERE Id = @PESSOAIDUPDATE";
                    command.Parameters.AddWithValue("NOME", pessoa.Nome);
                    command.Parameters.AddWithValue("CPFUPDATE", pessoa.Cpf);
                    command.Parameters.AddWithValue("PESSOAIDUPDATE", pessoaId);
                    command.ExecuteScalar();

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);

                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                        Console.WriteLine("  Message: {0}", ex2.Message);
                    }
                    return false;
                }
            }
        }

        */
    }
}

