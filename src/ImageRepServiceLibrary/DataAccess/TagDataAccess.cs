using Dapper;
using ImageRepServiceLibrary.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace ImageRepServiceLibrary.DataAccess
{
    public interface ITagDataAccess
    {
        public Task<int> InsertTagsAsync(IEnumerable<Tag> tags, DbTransaction transaction, SqlConnection connection);
        public Task<int> InsertSingleTagAsync(Tag tag);
        public Task<int> DeleteTagAsync(Guid tagGuid);
        public Task<IEnumerable<Tag>> GetTagsForImageAsync(Guid imageGuid, DbTransaction transaction, SqlConnection connection);
        public Task<int> DeleteTagsByImageAsync(Guid imageGuid, DbTransaction transaction, SqlConnection connection);
        public Task<IEnumerable<string>> GetDistinctTagsAsync();
    }
    public class TagDataAccess : ITagDataAccess
    {
        private readonly string _connectionString;
        public TagDataAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MainDatabase");
        }

        public async Task<int> DeleteTagAsync(Guid tagGuid)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    int sum = 0;
                    sum += await connection.ExecuteAsync(@"DELETE FROM [dbo].[Tags]
                        WHERE [TagId] = @TagId
                    ",
                        new
                        {
                            TagId = tagGuid
                        }, transaction: transaction);
                    transaction.Commit();
                    return sum;
                }
            }
        }

        public async Task<int> DeleteTagsByImageAsync(Guid imageTag, DbTransaction transaction, SqlConnection connection)
        {
            int sum = 0;
            sum += await connection.ExecuteAsync(@"
                DELETE FROM [dbo].[Tags]
                    WHERE [ImageKey] = @ImageKey
                ", 
                new
                {
                    ImageKey = imageTag
                }, 
                transaction: transaction);
            return sum;
        }


        public async Task<IEnumerable<Tag>> GetTagsForImageAsync(Guid imageGuid, DbTransaction transaction, SqlConnection connection)
        {
            return await connection.QueryAsync<Tag>(@"select * FROM [dbo].[Tags]
                        WHERE [ImageKey] = @Id ORDER BY [Name] ASC",
                        new
                        {
                            Id = imageGuid
                        }, transaction: transaction);
        }

        public async Task<IEnumerable<string>> GetDistinctTagsAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    return await connection.QueryAsync<string>(@"SELECT DISTINCT Name from tags ORDER BY Name ASC", transaction: transaction);
                }
            }
        }

        public async Task<int> InsertSingleTagAsync(Tag tag)
        {
            int sum = 0;
            TagDataVerification.VerifyTagParameteres(tag);
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    sum += await connection.ExecuteAsync(@"
                        INSERT INTO [dbo].[Tags]
                               ([TagId]
                               ,[Name]
                               ,[ImageKey])
                         VALUES
                               (@TagId,
                                @Name,
                                @ImageKey)",
                    new
                    {
                        tag.TagId,
                        tag.Name,
                        tag.ImageKey
                    }, transaction: transaction);
                    transaction.Commit();
                }
            }
            return sum;
        }

        public async Task<int> InsertTagsAsync(IEnumerable<Tag> tags, DbTransaction transaction, SqlConnection connection)
        {
            if (tags is null)
            {
                return 0;
            }

            int sum = 0;
            foreach (var item in tags)
            {
                TagDataVerification.VerifyTagParameteres(item);

                sum += await connection.ExecuteAsync(@"
                    INSERT INTO [dbo].[Tags]
                               ([TagId]
                               ,[Name]
                               ,[ImageKey])
                         VALUES
                               (@TagId,
                                @Name,
                                @ImageKey)",
                new
                {
                    item.TagId,
                    item.Name,
                    item.ImageKey
                }, transaction: transaction);
            }

            return sum;
        }

        private static class TagDataVerification
        {
            public static void VerifyTagParameteres(Tag tag)
            {
                if (tag.Name == null || tag.Name == "")
                {
                    throw new TagDataException("Tag name is not valid.");
                }
                if (tag.TagId == null)
                {
                    throw new ImageDataException("Tag Id is not valid.");
                }
            }
        }


        


    }

    public class TagDataException : DBException
    {
        public TagDataException()
        { }

        public TagDataException(string message)
            : base(message)
        { }

        public TagDataException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
