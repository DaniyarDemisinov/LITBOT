using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace LITBOT
{
    public class DapperBookRepository
    {
        private NpgsqlConnection _connection;
        private const string _connectionString = "Server=localhost;Port=5432;User Id=postgres;Password=6p9xsyrqgc;Database=litbot";
        public DapperBookRepository()
        {
            _connection = new NpgsqlConnection(_connectionString);
            _connection.Open();
        }
        public async Task InsertToGenre(Genre genre)
        {
            if (await SearchRepeat("genre", "name", genre.name))
            {
                Console.WriteLine("Мы в SearchRepeat Genre");
                return;
            }
            string commandText = 
                "INSERT INTO genre (name) VALUES (@name)";
            var queryArguments = new
            {
                name = genre.name
            };
            await _connection.ExecuteAsync(commandText, queryArguments);
        }
        
        public async Task InsertToAuthor(Author author, int genreId)
        {
            if (await SearchRepeat("author", "name", author.name, genreId))
            {
                Console.WriteLine("Мы в SearchRepeat Author");
                return;
            }
            string commandText = "INSERT INTO author " +
                "(name, genreId) " +
                "VALUES " +
                "(@name, @genreId) ";
            var queryArguments = new
            {
                name = author.name,
                genreId = genreId
            };
            await _connection.ExecuteAsync(commandText, queryArguments);
        }
        //public async Task<int> GetLastId(string tableName)
        //{
        //    string text =
        //        $"SELECT " +
        //        $"id " +
        //        $"FROM {tableName} " +
        //        $"ORDER BY id DESC " +
        //        $"LIMIT @a";
        //    var queryArguments = new
        //    {
        //        a = 1
        //    };
        //    var result = await _connection.ExecuteAsync(text, queryArguments);
        //    return result;
        //}

        public async Task InsertToBook(Book book, int genreId, int authorId)
        {
            if (await SearchRepeat("book", "name", book.name, genreId, authorId))
            {
                Console.WriteLine("Мы в SearchRepeat Book");
                return;
            }
            string commandText = "INSERT INTO book " +
                "(name, picturePath, bookPath, genreId, authorId) " +
                "VALUES " +
                "(@name, @picturePath, @bookPath, @genreId, @authorId) ";
            var queryArguments = new 
            {
                name = book.name,
                picturePath = book.picturePath,
                bookPath = book.bookPath,
                genreId = genreId,
                authorId = authorId
            };
            await _connection.ExecuteAsync(commandText, queryArguments);
        }
        public async Task<int> GetId(string tableName, string columnName, string txt)
        {
            string commandText =
                $"SELECT id FROM {tableName} " +
                $"WHERE {columnName} = '{txt}'";

            var result = await _connection.QueryFirstOrDefaultAsync<int>(commandText);
            return result;
        }
        public async Task<bool> SearchRepeat(string tableName, string columnName, string txt)
        {
            string commandText =
                $"SELECT {columnName} FROM {tableName} " +
                $"WHERE {columnName} = '{txt}'";
            var result = await _connection.QueryFirstOrDefaultAsync<string>(commandText);
            if (result == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<bool> SearchRepeat(string tableName, string columnName, string txt, int genreId)
        {
            string commandText =
                $"SELECT {columnName} FROM {tableName} " +
                $"WHERE {columnName} = '{txt}' " +
                $"AND genreId = {genreId}";
            var result = await _connection.QueryFirstOrDefaultAsync<string>(commandText);
            if (result == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<bool> SearchRepeat(string tableName, string columnName, string txt, int genreId, int authorId)
        {
            string commandText =
                $"SELECT {columnName} FROM {tableName} " +
                $"WHERE {columnName} = '{txt}' " +
                $"AND genreId = {genreId} " +
                $"AND authorId = {authorId}";
            var result = await _connection.QueryFirstOrDefaultAsync<string>(commandText);
            if (result == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<IEnumerable<Book>> GetBooks(string nameBook)
        {
            string commandText = 
                $"SELECT id, name, picturePath, bookPath, genreId, authorId FROM book " +
                $"WHERE name = '{nameBook}'";
            var result = await _connection.QueryAsync<Book>(commandText);
            return result;
        }
        public async Task<string> GetName(string tableName, int id)
        {
            string commandText = 
                $"SELECT name FROM {tableName} " +
                $"WHERE id = {id}";
            var result = await _connection.QueryFirstOrDefaultAsync<string>(commandText);
            return result;
        }
    }
}
