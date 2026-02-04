using System.Collections.ObjectModel;
using static System.Net.WebRequestMethods;

namespace MultiTuner.ViewModel.TestUiViewModel
{
    internal class MainWindowViewModel
    {
        public ObservableCollection<TestPlaylist> Playlist { get; set; }

        public ObservableCollection<TestPlaylist> Playlist2 { get; set; }

        public ObservableCollection<SongList> SongList { get; set; }


        public ObservableCollection<SongList> SongList2 { get; set; }



        public MainWindowViewModel()
        {
            Playlist = new ObservableCollection<TestPlaylist>()
            {
                new TestPlaylist() { Name = "Playlist 1", ImageUrl = "https://www.teachhub.com/wp-content/uploads/2019/10/Our-Top-10-Songs-About-School-768x569.png" }, 
                new TestPlaylist() { Name = "Playlist 2", ImageUrl = "https://m.timesofindia.com/photo/123016129/size-161202/123016129.jpg"},
                new TestPlaylist() { Name = "Playlist 3", ImageUrl = "https://cdn.prod.website-files.com/655e0fa544c67c1ee5ce01c7/655e0fa544c67c1ee5ce0f07_spotify-playlist-followers-FB_large_5b7c1f379b14e.webp" },
                new TestPlaylist() { Name = "Playlist 4", ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ0rI6alF_iOCtgk9eNQZxD73p0vWmYX7VpQA&s"},
                new TestPlaylist() { Name = "Playlist 5", ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRhSLtb7LPASd75Vy1ErFVg39JEnW6jJ2rNYg&s"},
                new TestPlaylist() { Name = "Playlist 6", ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ6-3JHK-GtrtJ-JfoI42RAOzyd13YUJGf3Eg&s"},
            };



            Playlist2= new ObservableCollection<TestPlaylist>()
            {
                new TestPlaylist() { Name = "Playlist 1", ImageUrl = "https://i.scdn.co/image/ab67616d00001e02ad2bf6cb7e74f83550cee265" },
                new TestPlaylist() { Name = "Playlist 2", ImageUrl = "https://i.ytimg.com/vi/JYuyWrkwpok/maxresdefault.jpg"},
                new TestPlaylist() { Name = "Playlist 3", ImageUrl = "https://upload.wikimedia.org/wikipedia/he/f/f2/Boney_M._-_Daddy_Cool_%281976_single%29.jpg"},
            };


            SongList = new ObservableCollection<SongList>()
            {
                new SongList() {ImageUrl = "https://i.ytimg.com/vi/vkOJ9uNj9EY/maxresdefault.jpg",Name = "Song 1", Artist = "Artist 1", Duration = "3:45" },
                new SongList() {ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS0v20d2TtGqcXwVDlIhrFCh0yW3U0lKiY0KA&s", Name = "Song 2", Artist = "Artist 2", Duration = "4:20" },
                new SongList() {ImageUrl = "https://m.media-amazon.com/images/I/81KfamwxPCL._AC_UF1000,1000_QL80_.jpg", Name = "Song 3", Artist = "Artist 3", Duration = "2:55" },
                new SongList() {ImageUrl = "https://upload.wikimedia.org/wikipedia/en/6/69/Eminem_-_The_Real_Slim_Shady_CD_cover.jpg", Name = "Song 4", Artist = "Artist 4", Duration = "5:10" },
            };


            SongList2 = new ObservableCollection<SongList>()
            {
                new SongList() {ImageUrl = "https://upload.wikimedia.org/wikipedia/en/6/69/Eminem_-_The_Real_Slim_Shady_CD_cover.jpg", Name = "Song 1", Artist = "Artist 1", Duration = "3:45" },
                new SongList() {ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSQ9bnF1fWjkihx3tiyu6y14BmPd4ea-Q6kUQ&s", Name = "Song 2", Artist = "Artist 2", Duration = "4:20" },
                new SongList() {ImageUrl = "https://i.ytimg.com/vi/B21s7zHjZOo/hq720.jpg?sqp=-oaymwEhCK4FEIIDSFryq4qpAxMIARUAAAAAGAElAADIQj0AgKJD&rs=AOn4CLA8Ord64F9kMK21gUUFF0WVdd4c_w", Name = "Song 3", Artist = "Artist 3", Duration = "2:55" },
                new SongList() {ImageUrl = "https://i.etsystatic.com/33735758/r/il/347b0b/5115214520/il_fullxfull.5115214520_a967.jpg", Name = "Song 4", Artist = "Artist 4", Duration = "5:10" },
            };



        }
    }

    public class TestPlaylist
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        
    }



    public class SongList : TestPlaylist
    {
        public string Artist { get; set; }      // New
        public string Duration { get; set; }    // New

    }

}
