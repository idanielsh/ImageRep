using ImageRepServiceLibrary.DataAccess;
using ImageRepServiceLibrary.Model;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace ImageRepServiceLibraryTests.DataAccess
{
    public class ImageDataAccessTests
    {
        private readonly bool _commiteToDB = false;

        public static ImageDataAccess Setup()
        {
            var configurationSectionMock = new Mock<IConfigurationSection>();
            configurationSectionMock
                .SetupGet(m => m[It.Is<string>(s => s == "MainDatabase")])
                .Returns("Data Source=host.docker.internal,5050; Initial Catalog=ImageRep;User Id=sa;Password=P@assword123;MultipleActiveResultSets=true");

            var configurationMock = new Mock<IConfiguration>();
            configurationMock
                .Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings")))
                .Returns(configurationSectionMock.Object);

            var recipeRepository = new ImageDataAccess(new TagDataAccess(configurationMock.Object), configurationMock.Object);
            return recipeRepository;
        }

        [Fact]
        public async Task InsertImage()
        {
            IImageDataAccess imageDataAccess = Setup();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var id = Guid.NewGuid();
                var rows = await imageDataAccess.SaveImageAsync(new ImageRepServiceLibrary.Model.Image()
                {
                    Id = id,
                    Name = "Bob",
                    Description = "A picture of Bob",
                    Picture = "dfdsgfs",
                    DateCreated = DateTime.Now,
                    Tags = new List<Tag>()
                    {
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Person",
                            ImageKey = id
                        },
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Friend",
                            ImageKey = id
                        }
                    }
                });

                Assert.Equal(3, rows);



                id = Guid.NewGuid();
                rows = await imageDataAccess.SaveImageAsync(new ImageRepServiceLibrary.Model.Image()
                {
                    Id = id,
                    Name = "Ozzy the puppy",
                    Description = "A picture of Ozzy when he was 3 months old",
                    Picture = "vdfgdgfdsf",
                    DateCreated = DateTime.Now
                });

                Assert.Equal(1, rows);
                if (_commiteToDB)
                {
                    scope.Complete();
                }
            }
        }

        /// <summary>
        /// IMPORTANT: This test will only work correctly when there is no other elements with 
        /// names/tags/description similar to the ones returned in GenerateImages()
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SearchImages()
        {
            IImageDataAccess imageDataAccess = Setup();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var imageList = GenerateImages();

                foreach (var item in imageList)
                {
                    await imageDataAccess.SaveImageAsync(item);
                }

                var imagesFromDb = await imageDataAccess.SearchImagesAsync(false, tagName: "Dog");
                Assert.Equal(2, imagesFromDb.Count());

                imagesFromDb = await imageDataAccess.SearchImagesAsync(false, name: "png");
                Assert.Equal(2, imagesFromDb.Count());

                imagesFromDb = await imageDataAccess.SearchImagesAsync(false, description: " ");
                Assert.Equal(4, imagesFromDb.Count());


                imagesFromDb = await imageDataAccess.SearchImagesAsync(false, name: "png", tagName: "Dog");
                Assert.Equal(2, imagesFromDb.Count());

                imagesFromDb = await imageDataAccess.SearchImagesAsync(true, name: "png", tagName: "dog ", description: "family");
                Assert.Single(imagesFromDb);
                Assert.Equal(imagesFromDb.First(), imageList[2]);

                if (_commiteToDB)
                {
                    scope.Complete();
                }

            }
        }

        [Fact]
        public async Task InsertImages()
        {
            IImageDataAccess imageDataAccess = Setup();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var imageList = GenerateImages();

                int rows = 0;

                foreach (var item in imageList)
                {
                    rows += await imageDataAccess.SaveImageAsync(item);
                }
                Assert.Equal(21, rows);

                if (_commiteToDB)
                {
                    scope.Complete();
                }
            }
        }

        [Fact]
        public async Task RemoveImage()
        {
            IImageDataAccess imageDataAccess = Setup();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var id = Guid.NewGuid();
                await imageDataAccess.SaveImageAsync(new ImageRepServiceLibrary.Model.Image()
                {
                    Id = id,
                    Name = "Bob",
                    Description = "A picture of Bob",
                    Picture = "dfdsgfs",
                    DateCreated = DateTime.Now,
                    Tags = new List<Tag>()
                    {
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Person",
                            ImageKey = id
                        },
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Friend",
                            ImageKey = id
                        }
                    }
                });

                int count = await imageDataAccess.DeleteImageAsync(id);

                Assert.Equal(3, count);

                id = new Guid();
                count = await imageDataAccess.DeleteImageAsync(id);
                Assert.Equal(0, count);


                if (_commiteToDB)
                {
                    scope.Complete();
                }
            }
        }

        [Fact]
        public async Task LoadSingleImage()
        {
            IImageDataAccess imageDataAccess = Setup();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var id = Guid.NewGuid();
                Image testImage = new Image()
                {
                    Id = id,
                    Name = "Bob",
                    Description = "A picture of Bob",
                    Picture = "dfdsgfs",
                    DateCreated = DateTime.Now,
                    Tags = new List<Tag>()
                    {
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Person",
                            ImageKey = id
                        },
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Friend",
                            ImageKey = id
                        }
                    }
                };
                await imageDataAccess.SaveImageAsync(testImage);

                Image imageFromDb = await imageDataAccess.GetImageAsync(id, false);
                Assert.Equal(0, imageFromDb.Tags.Count);
                Assert.True(compareImages(imageFromDb, testImage));

                imageFromDb = await imageDataAccess.GetImageAsync(id, true);
                Assert.Equal(2, imageFromDb.Tags.Count);
                Assert.True(compareImages(imageFromDb, testImage));

                id = Guid.NewGuid();
                testImage = new Image()
                {
                    Id = id,
                    Name = "Bob",
                    Description = "A picture of Bob",
                    Picture = "dfdsgfs",
                    DateCreated = DateTime.Now,
                };
                await imageDataAccess.SaveImageAsync(testImage);

                imageFromDb = await imageDataAccess.GetImageAsync(id, true);
                Assert.Equal(0, imageFromDb.Tags.Count);

                if (_commiteToDB)
                {
                    scope.Complete();
                }
            }
        }

        


        /// <summary>
        /// Compares all parameters of the 2 images. Different from implementation 
        /// in Image class since other implementation only compares Id.
        /// This implementation does not compare tags.
        /// </summary>
        /// <param name="image1">First image to compare</param>
        /// <param name="image2">Second image to compare</param>
        /// <returns>True if all parameters (except list of tags) are the same</returns>
        private bool compareImages(Image image1, Image image2)
        {
            bool compare = image2.Id.Equals(image1.Id) &&
                   image2.Name == image1.Name &&
                   image2.Description == image1.Description &&
                   image2.Picture == image1.Picture &&
                   CompareRoundedDates(image1.DateCreated, image2.DateCreated);
            return compare;
        }

        private bool CompareRoundedDates(DateTime date1, DateTime date2)
        {
            return date1.Year == date2.Year &&
                   date1.Date == date2.Date &&
                   date1.Hour == date2.Hour &&
                   date1.Minute == date2.Minute &&
                   date1.Second == date2.Second;
        }

        private IList<Image> GenerateImages()
        {
            var Id1 = Guid.NewGuid();
            var testImage1 = new Image()
            {
                Id = Id1,
                Name = "e32rwsgrfg.jpg",
                Description = "A great image of the mountains",
                Picture = "asdfghjklpoiuytrewqasxdcfgvjhbkjnbutyfr",
                Tags = new List<Tag>()
                    {
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Mountain",
                            ImageKey = Id1
                        },
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Skyline",
                            ImageKey = Id1
                        },
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Nature",
                            ImageKey = Id1
                        },
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Wildlife",
                            ImageKey = Id1
                        }
                    }
            };

            var Id2 = Guid.NewGuid();
            var testImage2 = new Image()
            {
                Id = Id2,
                Name = "gs432sdfhs.png",
                Description = "A picture of my dog, Ozzy",
                Picture = "asdfghjklpoiuytrewqasxdcfgvjhbkjnbutyfr",
                Tags = new List<Tag>()
                    {
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Dog",
                            ImageKey = Id2
                        },
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Puppy",
                            ImageKey = Id2
                        },
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Golden Retriever",
                            ImageKey = Id2
                        },
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Toy",
                            ImageKey = Id2
                        },
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Cutie Pie",
                            ImageKey = Id2
                        }
                    }
            };

            var Id3 = Guid.NewGuid();
            var testImage3 = new Image()
            {
                Id = Id3,
                Name = "mj2kfwe343.png",
                Description = "A picture of our family",
                Picture = "asdfghjklpoiuytrewqasxdcfgvjhbkjnbutyfr",
                Tags = new List<Tag>()
                    {
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Family",
                            ImageKey = Id3
                        },
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Dog",
                            ImageKey = Id3
                        },
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Parents",
                            ImageKey = Id3
                        }

                    }
            };

            var Id4 = Guid.NewGuid();
            var testImage4 = new Image()
            {
                Id = Id4,
                Name = "2g7sdfrgre.jpeg",
                Description = "A picture of my house",
                Picture = "asdfghjklpoiuytrewqasxdcfgvjhbkjnbutyfr",
                Tags = new List<Tag>()
                    {
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "House",
                            ImageKey = Id4
                        },
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Suburbs",
                            ImageKey = Id4
                        },
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Door",
                            ImageKey = Id4
                        },
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Windows",
                            ImageKey = Id4
                        },
                        new Tag()
                        {
                            TagId = Guid.NewGuid(),
                            Name = "Car",
                            ImageKey = Id4
                        },

                    }
            };
            return new List<Image>()
            {
                testImage1,testImage2,testImage3,testImage4
            };
        }


    }
}
