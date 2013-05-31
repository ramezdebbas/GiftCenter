using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace FoodVariable.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : FoodVariable.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, int colSpan, int rowSpan, SampleDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._colSpan = colSpan;
            this._rowSpan = rowSpan;
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private int _rowSpan = 1;
        public int RowSpan
        {
            get { return this._rowSpan; }
            set { this.SetProperty(ref this._rowSpan, value); }
        }

        private int _colSpan = 1;
        public int ColSpan
        {
            get { return this._colSpan; }
            set { this.SetProperty(ref this._colSpan, value); }
        }


        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> TopItems
        {
            get { return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");

            return _sampleDataSource.AllGroups;
        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SampleDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }



        public SampleDataSource()
        {
            String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                        "In Pakistan and India every woman knows that bride is incomplete without applying beautiful and stunning mehndi designs on her hands, feet and arms. These days, applying mehndi becomes popular fashion. Every year, numerous mehndi designs are coming for women and young girls. In this post, we are presenting latest and exclusive mehndi designs 2013 for women. Women and girls can apply these mehndi designs on their hands, arms and feet. All mehndi designs 2013 are simply stunning and magnificent. These mehndi designs 2013 include different types of designs like floral designs, peacock designs and many more. If we talk about these mehndi designs then some mehndi designs are extremely beautiful but difficult. So women can apply them with the help of professional mehndi artist. On the other hand, some of them are simple then even girls can easily apply them without taking any help.");

            var group1 = new SampleDataGroup("Group-1",
                 "Baby Shower Goods",
                 "Group Subtitle: 1",
                 "Assets/DarkGray.png",
                 "Group Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs. In this style we can use different styles of mehndi like Black mehndi is used as outline, fillings with the normal henna mehndi. We can also include sparkles as a final coating to make the henna design more attractive.");

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item1",
                 "Gift 01",
                 "Gift 01",
                 "Assets/HubPage/HubpageImage2.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "Traditionally, baby showers were given only for the family's first child, and only women were invited. The original intent was for women to share wisdom and lessons on the art of becoming a mother.Over time, it has become common to hold them for subsequent or adopted children. It is not uncommon for a parent to have more than one baby shower, such as one with friends and another with co-workers.",
                 35,
                 35,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item2",
                 "Gift 02",
                 "Gift 02",
                 "Assets/HubPage/HubpageImage3.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "Baby showers are an alternative to other European celebrations of nativity such as Baptisms. However, these can tend to be less materialistic as what is commonly known as a baby shower in the twenty-first century.\n\nAccording to etiquette authority Miss Manners, because the party centers on gift-giving, the baby shower is typically arranged and hosted by a close friend rather than a member of the family, since it is considered rude for families to beg for gifts on behalf of their members.[2] However, this custom varies by culture or region and in some it is expected and customary for a close female family member to host the baby shower, often the grandmother.",
                 35,
                 35,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item3",
                 "Gift 03",
                 "Gift 03",
                 "Assets/HubPage/HubpageImage4.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "There is no set rule for when or where showers are to be held. The number of guests and style of entertainment are determined by the host. Most hosts invite only women to baby showers, although there is no firm rule requiring this. If the shower is held after the baby's birth, then the baby is usually brought, too. Showers typically include food but not a full meal.Some hosts arrange baby-themed activities, such as games to taste baby foods or to guess the baby's birth date or sex.",
                 35,
                 35,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item4",
                 "Gift 04",
                 "Gift 04",
                 "Assets/HubPage/HubpageImage5.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "The absolute best gifts for a 1 year old boy are all here in this huge list, complete with product reviews--and all are 100% kid-approved! It's so hard to find gifts and toys for 1 year old boys that are fun, educational, and are--most of all--safe this was always my biggest concern!). It's always a bonus, too, if you can find a gift that the little guy can grow up and into.",
                 69,
                 70,
                 group1));

            group1.Items.Add(new SampleDataItem("Landscape-Group-1-Item5",
                 "Gift 05",
                 "Gift 05",
                 "Assets/HubPage/HubpageImage6.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "I can't being to tell you how cute this rocking caterpillar is--I hate to keep it in the corner of his room. We bought this when our son was about nine months, and he's still using it now at almost 18 months. It's an adorable rocker that can be removed from the base to make it into a soft toy. It's PERFECT for lounging on--he loves to cuddle and roll around with it.",
                 69,
                 35,
                 group1));

            

            this.AllGroups.Add(group1);

            var group2 = new SampleDataGroup("Group-2",
                "First Birthday",
                "Group Subtitle: 2",
                "Assets/DarkGray.png",
                "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");

            group2.Items.Add(new SampleDataItem("Big-Group-2-Item1",
                "Gift 01",
                "Gift 01",
                "Assets/HubPage/HubpageImage7.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "This is an awesome activity scooter for one year old boys--perhaps I'm partial since I'm from the midwest. The horn plays one of my son's favorite songs -- Old McDonald. He loves to hear the farm songs. It also will play (pretty realistic) tractor sounds. He can walk it from behind while younger; this bar is removable for when he solely uses it as a ride-on toy.",
                69,
                70,
                group2));

            group2.Items.Add(new SampleDataItem("Landscape-Group-2-Item2",
                "Gift 02",
                "Gift 02",
                "Assets/HubPage/HubpageImage8.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "I challenge you to find one little boy that doesn't love this toy. He'll sing songs, count, play animal sounds and so much more. It really melts my heart when Scout says, Will you give me a hug? and I love you! It's cute to hear my son now repeat them back to him and give him kisses.",
                69,
                35,
                group2));

            group2.Items.Add(new SampleDataItem("Medium-Group-2-Item3",
                "Gift 03",
                "Gift 03",
                "Assets/HubPage/HubpageImage9.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "My kiddo always have one of these Little People in his hand...and his pocket...and his bed. He's not attached to them by any means (not like a lovey or anything) but he just loves them because they fit so perfectly in his hands. He loves them *almost* as much as acorns, which is what he is collecting nowadays.",
                41,
                41,
                group2));

            group2.Items.Add(new SampleDataItem("Medium-Group-2-Item4",
                "Gift 04",
                "Gift 04",
                "Assets/HubPage/HubpageImage09.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "Any of these are super-cute, but I just can't get enough of that bouncing cow. So many toys for 1 year olds are bright and colorful; I kind of like the fact that the cows are more subdued. It's the kind of toy that I don't get anxious leaving out in the house after everything's put away. It's also great if you prefer a more modern look to toys.",
                41,
                41,
                group2));

            
            this.AllGroups.Add(group2);


            

            var group3 = new SampleDataGroup("Group-3",
               "Toys",
               "Group Subtitle: 2",
               "Assets/DarkGray.png",
               "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");

            group3.Items.Add(new SampleDataItem("Big-Group-3-Item1",
                "Gift 01",
                "Gift 01",
                "Assets/HubPage/HubpageImage10.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "This is a very highly-rated toy that is sure to pique his curiosity. There are lights and sounds that he'll love, but he won't be able to resist launching his cars (called Wheelies -- cars that fit perfectly in little hands) down the huge ramp at high speeds. I promise you, he won't be able to resist pushing that launch button at the top of this brightly-covered toy!",
                69,
                70,
                group3));

            group3.Items.Add(new SampleDataItem("Landscape-Group-3-Item2",
                "Gift 02",
                "Gift 02",
                "Assets/HubPage/HubpageImage11.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "Anything with slides captures his attention--it's like he can't quite understand how something can go so fast. He'll get it someday, I'm sure. My husband also has an obsession with cars (too bad his are more expensive) and he loves that my son plays with these.",
                69,
                35,
                group3));

            group3.Items.Add(new SampleDataItem("Medium-Group-3-Item3",
                "Gift 03",
                "Gift 03",
                "Assets/HubPage/HubpageImage12.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "The table below is a great idea because these things--even though they are big--get everywhere. It's also great if you 1 year old boy will be playing with these on a carpeted surface. My son loves to play with these, but we typically play on rugs. EVERY TIME it falls over before he wants it to, he sobs. Playing on a harder surface makes it easier and avoids the meltdown!",
                41,
                41,
                group3));
            group3.Items.Add(new SampleDataItem("Medium-Group-3-Item4",
               "Gift 04",
               "Gift 04",
               "Assets/HubPage/HubpageImage13.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "This classic toy has been a favorite gift of 1 year old boys past and present! All little boys love sound, and this gift creates a fun poppity-pop sound which encourages young walkers to keep walking. It may not be music to parents' ears, but this fun popping sound is all the encouragement your little guy needs to keep walking.",
               41,
               41,
               group3));

            this.AllGroups.Add(group3);


         



            var group4 = new SampleDataGroup("Group-4",
               "Some Important Gifts",
               "Group Subtitle: 2",
               "Assets/DarkGray.png",
               "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group4.Items.Add(new SampleDataItem("Medium-Group-4-Item1",
               "Gift 01",
               "Gift 01",
               "Assets/HubPage/HubpageImage14.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "This bright and cheerful solid wooden cube contains lots of playful activities including curvy bead mazes, peek-a-boo-doors, spin and match animals, turn and learn ABC tiles and racing vehicle rollers. My son was 10 months when we got this, and he LOVES this toy. It's sizeable (16 h. x 12 w. x 12), and I think that really captured his attention. He can either sit and play with it or stand--perfect for developing both mind and body strength.",
               41,
               41,
               group4));

            group4.Items.Add(new SampleDataItem("Medium-Group-4-Item2",
                "Gift 02",
                "Gift 02",
                "Assets/HubPage/HubpageImage15.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "Part toy, part sculpture. It's a cool toy that he'll love and you won't actually mind looking at. It's been amazing to see my son play with these over the past year. At first he just liked to spin the beads, and then he realized he could actually MOVE them. Then, he put two and two together that moving more than one bead is kind of like a choo-choo-train. It's so rewarding to watch their brains develop!",
                41,
                41,
                group4));
            group4.Items.Add(new SampleDataItem("Medium-Group-4-Item3",
               "Gift 03",
               "Gift 03",
               "Assets/HubPage/HubpageImage16.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "One thing my son particularly loves is the song that is all about his favorite things, which we programmed in using the USB cord that comes in the package. Scout sings, I'd like A COW...that's my favorite animal. I'd like it to be BLUE...that's my favorite color. I'd like it to eat BANANAS...that's my favorite food. And I'll call it JACK...just like you.So cute!",
               41,
               41,
               group4));
            group4.Items.Add(new SampleDataItem("Medium-Group-4-Item4",
               "Gift 04",
               "Gift 04",
               "Assets/HubPage/HubpageImage17.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "This is the best book ever. Let me repeat -- this is the BEST. BOOK. EVER. Why, you ask? Because not only is it personalized (and therefore, adorable), but it's known as a quiet book. These fabric books are super-soft and perfect for occasions like going out to dinner. Since it's soft, your one year old can't bang it on tables and cause a ruckus. It's got fun activities inside -- he'll learn to fasten a buckle, tie a shoe, etc., so it'll keep him occupied for more than just a few minutes. ",
               41,
               41,
               group4));
            this.AllGroups.Add(group4);



        }
    }
}
