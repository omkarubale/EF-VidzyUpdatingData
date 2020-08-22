using System;
using System.Data.Entity;
using System.Linq;

namespace Vidzy
{
    class Program
    {
        static void Main(string[] args)
        {
            //AddVideo("Terminator 1", "Action", new DateTime(1984, 10, 26), Classification.Silver);
            //AddTag("Classics");
            //AddTag("Drama");
            //AddTagsToVideo(1, "Classics", "Drama", "Comedy");
            //DeleteTagsFromVideo(1, "Comedy");
            //DeleteVideo(1);
            DeleteGenreAndLinkedVideos(2);
        }

        public static void AddVideo(string name, string genreName, DateTime releaseDate, Classification classification)
        {
            using (var db = new VidzyContext())
            {
                var genreId = db.Genres.SingleOrDefault(g => g.Name == genreName).Id;

                var video = new Video()
                {
                    Name = name,
                    GenreId = genreId,
                    ReleaseDate = releaseDate,
                    Classification = classification
                };

                db.Videos.Add(video);

                db.SaveChanges();
            }
        }

        public static void DeleteVideo(int videoId)
        {
            using(var db = new VidzyContext())
            {
                var video = db.Videos.Single(v => v.Id == videoId);

                db.Videos.Remove(video);

                db.SaveChanges();
            }
        }

        public static void AddTag(string name)
        {
            using (var db = new VidzyContext())
            {
                var tag = new Tag()
                {
                    Name = name,
                };

                db.Tags.Add(tag);

                db.SaveChanges();
            }
        }

        public static void AddTagsToVideo(int videoId, params string[] tags)
        {
            using (var db = new VidzyContext())
            {
                var video = db.Videos.SingleOrDefault(v => v.Id == videoId);

                var tagsInDb = db.Tags.ToList();

                foreach (var tag in tags)
                {
                    var tagInDb = tagsInDb.SingleOrDefault(t => t.Name == tag);
                    if (tagInDb == null)
                        video.Tags.Add(new Tag() { Name = tag });
                    else
                        video.Tags.Add(tagInDb);
                }

                db.SaveChanges();
            }
        }

        public static void DeleteTagsFromVideo(int videoId, params string[] tags)
        {
            using (var db = new VidzyContext())
            {
                var video = db.Videos.Include(v => v.Tags).SingleOrDefault(v => v.Id == videoId);

                var tagsInDb = db.Tags.Where(t => tags.Contains(t.Name)).ToList();

                foreach(var tag in tagsInDb)
                    video.Tags.Remove(tag);

                db.SaveChanges();
            }
        }

        public static void DeleteGenreAndLinkedVideos(int genreId)
        {
            using(var db = new VidzyContext())
            {
                var genre = db.Genres.Include(g => g.Videos).Single(g => g.Id == genreId);
                var videos = db.Videos.Where(v => v.GenreId == genreId);

                db.Videos.RemoveRange(videos);
                db.Genres.Remove(genre);

                db.SaveChanges();
            }
        }
    }
}
