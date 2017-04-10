using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using GeoAPI;
//using Xamarin.Forms.Maps;

namespace WhereAmI
{
    public class App : Application
    {
        IGeolocator locator { get; } = CrossGeolocator.Current;
        GeofenceManager geofenceManager { get; } = GeofenceManagerProvider.Instance;
        bool automaticUpdates { get; set; } = false;
        public App()
        {
            GeoAPI.NetTopologySuiteBootstrapper.Bootstrap();

            btnGetLocation.Clicked += BtnGetLocation_Clicked;

            btnToggleLocationUpdates.Clicked += BtnToggleLocationUpdates_Clicked;

            btnShowOnMap.Clicked += BtnShowOnMap_Clicked;

            geofenceManager.OnEnteredGeofence += GeofenceManager_OnEnteredGeofence;
            geofenceManager.OnExitedGeofence += GeofenceManager_OnExitedGeofence;

            // Todo different screen.. probably a table for this PoC
            //Geofence homeFence = new Geofence("Home", 40.596625, -75.591912, 50);
            //Geofence workFence = new Geofence("Work", "POLYGON((-75.56702256202698 40.60780962922667,-75.56735515594482 40.60706841686434,-75.56759119033813 40.6062213069555,-75.56657195091248 40.60672631607828,-75.56594967842102 40.606986964455075,-75.56629300117493 40.60737793511422,-75.56671142578125 40.60783406465907,-75.56702256202698 40.60780962922667))");
            //Geofence churchRdAndChapmansFence = new Geofence("Home to Work Checkpoint", 40.607447, -75.572532, 10);
            //Geofence allentownYMCAFence = new Geofence("Allentown YMCA", "POLYGON((-75.48591256141663 40.593485683191474,-75.48546195030212 40.593603813008954,-75.48523128032684 40.59307833739258,-75.48567652702332 40.59295205969104,-75.48591256141663 40.593485683191474))");
            //Geofence tilghmanFence = new Geofence("Tilghman Street From LTI to Wegmans", "LINESTRING(-75.57602405548101 40.59273444649959,-75.5604887008667 40.58914967802697,-75.55851459503174 40.58918226769682,-75.55048942565918 40.591430916562736,-75.54040431976318 40.5941683000797)", 20);

            //geofenceManager.SubscribeGeofence(homeFence);
            //geofenceManager.SubscribeGeofence(workFence);
            //geofenceManager.SubscribeGeofence(churchRdAndChapmansFence);
            //geofenceManager.SubscribeGeofence(allentownYMCAFence);
            //geofenceManager.SubscribeGeofence(tilghmanFence);

            geofenceManager.SubscribeGeofence(new RouteGeofence("Old U.S. 22/Tilghman Street", "LINESTRING(-75.63256502151489 40.583270292645814,-75.62968969345093 40.58387325197076,-75.62299489974976 40.58516063341827,-75.61393976211548 40.586985739332,-75.60361862182617 40.58918557758428,-75.5944561958313 40.59094539608536,-75.5893063545227 40.59192305301892,-75.58518648147583 40.59270516827414,-75.58329820632935 40.59298216523328,-75.5811095237732 40.59312881021768,-75.57900667190552 40.59309622247117,-75.57701110839844 40.592949577415276,-75.57471513748169 40.59260740436753,-75.5670976638794 40.590733568532386,-75.56078910827637 40.58925075687317,-75.55967330932617 40.58915298791604,-75.55870771408081 40.58921816723667,-75.55750608444214 40.58942999958997,-75.55413722991943 40.59032620596762,-75.54830074310303 40.591988229639604,-75.54461002349854 40.593047340821606,-75.54040431976318 40.59413902248207,-75.52789449691772 40.59759318135387)", 10));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Church Street", 40.58326545469004, -75.63260793685913, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Nursery Street", 40.58386026599528, -75.62973260879517, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Route 100", 40.58449581195547, -75.62643885612488, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Snowdrift Road", 40.58758382751916, -75.6109356880188, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Farm Bureau Road", 40.58983253013347, -75.60011029243469, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Blue Barn Road", 40.59191006860644, -75.58931708335876, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Broadway", 40.589188887471586, -75.56073546409607, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Hausman Road", 40.58940886730601, -75.55749535560608, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Rt 309 S", 40.589588109599035, -75.55518865585327, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Rt 309 N", 40.59016657008576, -75.55306434631348, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Rt 309 S", 40.59058208086843, -75.55538177490234, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Rt 309 N", 40.59117682710392, -75.55326819419861, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Parkway Road", 40.59162492008865, -75.549556016922, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("N 40th Street", 40.593001768834874, -75.54455637931824, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Springhouse Road", 40.594150478934836, -75.54040431976318, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("N Cedar Crest Blvd", 40.597596490824984, -75.52785158157349, 15));

            geofenceManager.SubscribeGeofence(new RouteGeofence("Rt 309", "LINESTRING(-75.58239698410034 40.628542298427305, -75.58188199996948 40.626702041356275, -75.58080911636353 40.62155556656084, -75.5805516242981 40.61820037264071, -75.58014392852783 40.61728825195737, -75.57947874069214 40.616343542406874, -75.57885646820068 40.61578974094238, -75.57735443115234 40.614975318684785, -75.5710244178772 40.61225507639128, -75.56840658187866 40.61108224315213, -75.56722640991211 40.6103492119241, -75.56617498397827 40.60927408491076, -75.5646300315857 40.6070911980673, -75.56220531463623 40.60365382126918, -75.55872917175293 40.59884771374569, -75.55596113204956 40.59495369867439, -75.55505990982056 40.593519861937054, -75.55460929870605 40.592265229568746, -75.55422306060791 40.59035879506413, -75.55357933044434 40.58654576299814, -75.55257081985474 40.58043468131494)", 10));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Lime Kiln Rd", 40.62851303588667, -75.58239698410034, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Hoffmansville Rd", 40.62677863492548, -75.58193564414978, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Village Rd", 40.62136343128806, -75.58081984519958, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Hucleberry Rd", 40.6192705206792, -75.58063745498657, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Snowdrift Rd", 40.61571975191955, -75.57868480682373, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Chapmans Rd/Pope Rd", 40.612266529739365, 75.57108879089355, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Walbert Ave", 40.61129731502486, -75.5688464641571, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Ridgeview Dr", 40.60707007136537, -75.56465148925781, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("US 22 W", 40.60026845343403, -75.56132555007935, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("US 22 E", 40.60109934686617, -75.55896520614624, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("US 22 E", 40.598215613618294, -75.5603814125061, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("US 22 W", 40.599421257340204, -75.55748462677002, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Tilghman Street E", 40.589588109599035, -75.55518865585327, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Tilghman Street E", 40.59016657008576, -75.55306434631348, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Tilghman Street W", 40.59058208086843, -75.55538177490234, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Tilghman Street W", 40.59117682710392, -75.55326819419861, 15));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("I-78", 40.580454288594815, 75.55254936218262, 15));

            geofenceManager.SubscribeGeofence(new RouteGeofence("I-476", "POLYGON((-75.2862811088562 40.10823901605814,-75.28561592102051 40.10710661693014,-75.27982234954834 40.109912962846224,-75.27774095535278 40.11204636472759,-75.27739763259888 40.11378585827243,-75.27774095535278 40.11580427143034,-75.27855634689331 40.117986715807824,-75.28164625167847 40.1218427432967,-75.28256893157959 40.123598393996964,-75.2843713760376 40.12837287826524,-75.2873969078064 40.136624890173984,-75.29003620147705 40.14446583380703,-75.29194593429565 40.15145302516492,-75.29286861419678 40.15347031993266,-75.29713869094849 40.158767483509884,-75.29986381530762 40.1620800507116,-75.30233144760132 40.16529407033854,-75.30434846878052 40.168065220198606,-75.30550718307495 40.17034436517593,-75.307137966156 40.17475486722441,-75.3108286857605 40.18306681862066,-75.31454086303711 40.19170557833722,-75.317223072052 40.19836011539604,-75.31988382339478 40.20416181428644,-75.32333850860596 40.210012177303796,-75.32683610916138 40.21599311884122,-75.33441066741943 40.22881835897025,-75.3438949584961 40.24510042395685,-75.34732818603516 40.25181855222938,-75.34868001937866 40.25452066236812,-75.3592586517334 40.27706679213357,-75.36904335021973 40.29849895768486,-75.37230491638184 40.30753208244466,-75.37668228149414 40.31957436979221,-75.38045883178711 40.330371333706644,-75.3823471069336 40.33573645517935,-75.38732528686523 40.34496084962916,-75.40328979492188 40.374261193366465,-75.40625095367432 40.378864528539964,-75.40801048278809 40.381623620411524,-75.40916919708252 40.38476184469688,-75.40972709655762 40.38780186005273,-75.41461944580078 40.41884128378311,-75.41547775268555 40.421919067837884,-75.41646480560303 40.4246307091371,-75.41839599609375 40.428354953949295,-75.42067050933838 40.43126233506559,-75.42998313903809 40.44334121872383,-75.44517517089844 40.46136746827328,-75.45221328735352 40.46986304933953,-75.45740604400635 40.47606583331193,-75.46289920806885 40.48367162306396,-75.46568870544434 40.48719676122763,-75.46697616577148 40.48863287554819,-75.46843528747559 40.48974257920259,-75.4711389541626 40.49170083500629,-75.47577381134033 40.493887486453325,-75.47976493835449 40.495127645158284,-75.48336982727051 40.49574771591513,-75.4885196685791 40.49651463760842,-75.49268245697021 40.49757525954749,-75.49530029296875 40.498945884603074,-75.49731731414795 40.50005541771012,-75.49933433532715 40.50227442887231,-75.50109386444092 40.505015458965524,-75.5024242401123 40.51010564624936,-75.50345420837402 40.5124222061227,-75.50637245178223 40.516011084507085,-75.51439762115479 40.52478671418955,-75.52233695983887 40.53378951191867,-75.53109169006348 40.5433455062987,-75.5416488647461 40.55537824930777,-75.54769992828369 40.562127378017735,-75.54907321929932 40.56372489826115,-75.55078983306885 40.56643081494913,-75.55508136749268 40.57386337247489,-75.56147575378418 40.584424007786616,-75.56859970092773 40.596286436604984,-75.56907176971436 40.596156091679546,-75.56353569030762 40.586900952451145,-75.5546522140503 40.572233496316805,-75.54975986480713 40.564050918116074,-75.5440092086792 40.5573345885268,-75.52894592285156 40.54041036296489,-75.51280975341797 40.522405337183955,-75.5049991607666 40.51356414280881,-75.50349712371826 40.51118236713017,-75.50178050994873 40.50573332928967,-75.50023555755615 40.50286180189317,-75.49808979034424 40.50041438037473,-75.49530029296875 40.498423745026564,-75.49105167388916 40.49653095499602,-75.48495769500732 40.495519269460885,-75.47959327697754 40.494638111565585,-75.47397136688232 40.492614668158595,-75.46873569488525 40.48931828291319,-75.46723365783691 40.48817593341592,-75.46504497528076 40.48582589616388,-75.46053886413574 40.479689299850676,-75.45783519744873 40.47593525429057,-75.42848110198975 40.44076761393917,-75.41928112506866 40.42825777960869,-75.41730299592018 40.42491760544373,-75.41541069746017 40.41889817352623,-75.40946289896965 40.383986927205115,-75.40849529206753 40.381710989489086,-75.3827665373683 40.33529695063302,-75.37030985578895 40.300541641485786,-75.35972560755908 40.27669114863423,-75.34894031938165 40.25388978933754,-75.3441699477844 40.244058594327186,-75.33862709999084 40.23468463802409,-75.32056510448456 40.20398071076179,-75.3083248436451 40.17550022874322,-75.30716143548489 40.17243845911039,-75.30606240034103 40.16978646722731,-75.30092395842075 40.16217812905058,-75.29479846358299 40.15509374448061,-75.29324848204851 40.15306014936443,-75.29232077300549 40.150846080825175,-75.28954485431314 40.14052489244393,-75.28595354408026 40.13046463375386,-75.28345657512546 40.12342198454031,-75.2820364292711 40.12127869735785,-75.27969557326287 40.118467699132246,-75.27865389129147 40.116307323713826,-75.27826179633848 40.114144057559564,-75.27858073299285 40.112635730028224,-75.27977626610664 40.11117554293739,-75.28122929128585 40.109912257277365,-75.28311146990745 40.10917410920024,-75.2862811088562 40.10823901605814))"));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Lehigh Valley (mile 56)", "POLYGON((-75.569908618927 40.59392885987243,-75.56689918041229 40.59344412130414,-75.5645763874054 40.592552030216424,-75.56424111127853 40.58917908511264,-75.56475341320038 40.58896725196446,-75.57003736495972 40.59282088083826,-75.57056307792664 40.59304084872484,-75.569908618927 40.59392885987243))"));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Quakertown (mile 44)", "POLYGON((-75.42505323886871 40.43624143091442,-75.4278963804245 40.43773885809469,-75.42807877063751 40.44003648144203,-75.42742431163788 40.440116095592465,-75.42502373456955 40.43859116201184,-75.42222619056702 40.436894709313606,-75.42294502258301 40.43633942307894,-75.42505323886871 40.43624143091442))"));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Lansdale (mile 31)", "POLYGON((-75.34968852996826 40.254815431492545,-75.34863710403442 40.25501194352883,-75.34456014633179 40.25122079814049,-75.34541845321655 40.25004983989829,-75.34728527069092 40.24959127730406,-75.34904479980469 40.251048840506904,-75.34968852996826 40.254815431492545))"));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("I-276 West", "POLYGON((-75.27971506118774 40.11367098742926,-75.27469396591187 40.11426174969865,-75.27510166168213 40.11342483496907,-75.28016567230225 40.112653551491164,-75.27971506118774 40.11367098742926))"));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("I-276 East", "POLYGON((-75.28151750564575 40.11183302755077,-75.27312755584717 40.113326373735525,-75.274178981781 40.11188225926633,-75.2784276008606 40.11058581220242,-75.28014421463013 40.11070068825656,-75.28143167495728 40.111209422735456,-75.28151750564575 40.11183302755077))"));
            geofenceManager.SubscribeGeofence(new TollboothGeofence("Mid-County (mile 20)", "POLYGON((-75.28645277023315 40.10825542750115,-75.28207540512085 40.10937139633457,-75.28175354003906 40.108972564173364,-75.28512239456177 40.1072936312206,-75.28599679470062 40.106995063038895,-75.28645277023315 40.10825542750115))"));


            mainStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.StartAndExpand,
                Padding = new Thickness(10, 20),
                Children = {
                    actIndGettingLocation,
                    lblTimeStamp,
                    lblLocation,
                    lblHeading,
                    lblAddress,
                    btnShowOnMap,
                    new StackLayout {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Children = {
                            new Label
                            {
                                Text = "Accuracy:",
                                HorizontalTextAlignment = TextAlignment.Start
                            },
                            entryDesiredAccuracy,
                            new Label
                            {
                                Text = " meters",
                                HorizontalTextAlignment = TextAlignment.Start
                            }
                        }
                    },
                    btnGetLocation,
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Children =
                        {
                            new Label
                            {
                                Text = "Update Interval:",
                                HorizontalTextAlignment = TextAlignment.Start
                            },
                            entryUpdateInterval,
                            new Label
                            {
                                Text = " seconds",
                                HorizontalTextAlignment = TextAlignment.Start
                            }
                        }
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Children =
                        {
                            new Label
                            {
                                Text = "Min Update Distance:",
                                HorizontalTextAlignment = TextAlignment.Center
                            },
                            entryMinimumDistance,
                            new Label
                            {
                                Text = " meters",
                                HorizontalTextAlignment = TextAlignment.Start
                            }
                        }
                    },
                    btnToggleLocationUpdates
                }
            };

            // The root page of your application
            var content = new ContentPage
            {
                Title = "WhereAmI",
                Content = new ScrollView
                {
                    Content = mainStack
                }
            };

            NavigationPage rootPage = new NavigationPage(content);
            content.ToolbarItems.Add(new ToolbarItem("Fences", String.Empty, async () =>
            {
                GeofenceCollectionView geofenceCollectionView = new GeofenceCollectionView();
                ContentPage geofencesPage = new ContentPage
                {
                    Content = geofenceCollectionView,
                    Title = "Geofences"
                };

                // Needed if modally presented
                //geofencesPage.ToolbarItems.Add(new ToolbarItem("Dismiss"), )

                geofencesPage.ToolbarItems.Add(new ToolbarItem("+", String.Empty, async () =>
                {
                    await geofencesPage.DisplayAlert("Create Geofence Not Implemented", "Working on it...", "F.U.");
                }));

                geofencesPage.Appearing += (s, e) => { geofenceCollectionView.RefreshCommand.Execute(null); };

                await rootPage.PushAsync(geofencesPage);
            }));


            MainPage = rootPage;
        }

        private async void BtnShowOnMap_Clicked(object sender, EventArgs e)
        {
            if (latestPosition == null)
            {
                await MainPage.DisplayAlert("No Latest Position", "Try again when location services has located you.", "Ok");
            }
            else
            {
                Xamarin.Forms.Maps.Map positionMap = new Xamarin.Forms.Maps.Map(
                    Xamarin.Forms.Maps.MapSpan.FromCenterAndRadius(
                        new Xamarin.Forms.Maps.Position(latestPosition.Latitude, latestPosition.Longitude), Xamarin.Forms.Maps.Distance.FromMeters(100)))
                {
                    MapType = Xamarin.Forms.Maps.MapType.Hybrid,
                };
                positionMap.Pins.Add(new Xamarin.Forms.Maps.Pin()
                {
                    Position = new Xamarin.Forms.Maps.Position(latestPosition.Latitude, latestPosition.Longitude),
                    Label = "YOU!",
                    Type = Xamarin.Forms.Maps.PinType.Generic
                });

                await MainPage.Navigation.PushAsync(new ContentPage
                {
                    Content = positionMap,
                    Title = "Map"
                });
            }
        }

        TollboothGeofence lastEnteredTollBooth { get; set; } = null;
        RouteGeofence currentRoute { get; set; } = null;

        private void GeofenceManager_OnEnteredGeofence(object sender, GeofenceEnteredEventArgs e)
        {
            //await MainPage.DisplayAlert("Exited Geofence!", e.Geofence.Name, "Ok");
            //mainStack.Children.Add(new Label
            //{
            //    HorizontalTextAlignment = TextAlignment.Start,
            //    HorizontalOptions = LayoutOptions.StartAndExpand,
            //    LineBreakMode = LineBreakMode.WordWrap,
            //    Text = $"Entered geofence {e.Geofence.Name} at {e.DateTimeEntered}"
            //});
            if (e.Geofence is TollboothGeofence)
            {
                lastEnteredTollBooth = e.Geofence as TollboothGeofence;
            }
            else if (e.Geofence is RouteGeofence)
            {
                // handle crossing over routes
                if (currentRoute == null)
                {
                    if (lastEnteredTollBooth != null)
                    {
                        currentRoute = e.Geofence as RouteGeofence;
                        AddMessageToUI($"Got on {currentRoute.Name} via the {lastEnteredTollBooth.Name} exit.");
                    }
                }
            }
        }

        private void GeofenceManager_OnExitedGeofence(object sender, GeofenceExitedEventArgs e)
        {
            //await MainPage.DisplayAlert("Entered Geofence!", e.Geofence.Name, "Ok");
            //mainStack.Children.Add(new Label
            //{
            //    HorizontalTextAlignment = TextAlignment.Start,
            //    HorizontalOptions = LayoutOptions.StartAndExpand,
            //    LineBreakMode = LineBreakMode.WordWrap,
            //    Text = $"Exited geofence {e.Geofence.Name} at {e.DateTimeExited}"
            //});
            if (e.Geofence is TollboothGeofence)
            {
                //lastEnteredTollBooth = e.Geofence as TollboothGeofence;
                lastEnteredTollBooth = null;
            }
            else if (e.Geofence is RouteGeofence)
            {
                if (e.Geofence == currentRoute)
                {
                    if (e.Geofence == null)
                    {
                        AddMessageToUI($"Got off {currentRoute.Name} via unspecified exit at ({LatestPosition.Latitude}, {LatestPosition.Longitude})");
                    }
                    else
                    {
                        AddMessageToUI($"Got off {currentRoute.Name} via the {lastEnteredTollBooth.Name} exit.");
                    }
                    currentRoute = null;
                    lastEnteredTollBooth = null;
                }
            }
        }

        private void AddMessageToUI(string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                mainStack.Children.Add(new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    LineBreakMode = LineBreakMode.WordWrap,
                    Text = message
                });
            });
        }

        private int GetLocatorAccuracy()
        {
            int accuracy;

            if (!Int32.TryParse(entryDesiredAccuracy.Text, out accuracy))
            {
                accuracy = defaultDesiredAccuracy;
            }

            return accuracy;
        }

        private int GetUpdateInterval()
        {
            int interval;
            if (!Int32.TryParse(entryUpdateInterval.Text, out interval))
            {
                interval = defaultUpdateInterval;
            }

            return interval;
        }

        private int GetMinimumDistance()
        {
            int distance;
            if (!Int32.TryParse(entryMinimumDistance.Text, out distance))
            {
                distance = defaultMinDistance;
            }

            return distance;
        }

        private async void BtnToggleLocationUpdates_Clicked(object sender, EventArgs e)
        {
            automaticUpdates = !automaticUpdates;

            // Automatically updating
            if (automaticUpdates)
            {
                locator.PositionChanged += Locator_PositionChanged;
                locator.PositionError += Locator_PositionError;
                locator.DesiredAccuracy = GetLocatorAccuracy();
                btnGetLocation.IsEnabled = false;
                entryUpdateInterval.IsEnabled = false;
                entryMinimumDistance.IsEnabled = false;
                entryDesiredAccuracy.IsEnabled = false;
                btnToggleLocationUpdates.Text = "Stop Updates";
                await locator.StartListeningAsync(
                    minTime: GetUpdateInterval(),
                    minDistance: GetMinimumDistance(),
                    includeHeading: true);
            }
            // Not automatically updating
            else
            {
                await locator.StopListeningAsync();
                btnGetLocation.IsEnabled = true;
                entryUpdateInterval.IsEnabled = true;
                entryMinimumDistance.IsEnabled = true;
                entryDesiredAccuracy.IsEnabled = true;
                btnToggleLocationUpdates.Text = "Start Updates";
                // unregister event handler so GC can collect reference
                locator.PositionChanged -= Locator_PositionChanged;
                locator.PositionError -= Locator_PositionError;
            }
        }

        Position latestPosition { get; set; }
        public Position LatestPosition => latestPosition;

        private void Locator_PositionChanged(object sender, PositionEventArgs e)
        {
            var position = e.Position;

            latestPosition = position;

            UpdateUIWithNewPosition(position);
            geofenceManager.UpdateGeofences(position);
        }

        private void Locator_PositionError(object sender, PositionErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Error);
        }

        private async void BtnGetLocation_Clicked(object sender, EventArgs e)
        {
            locator.DesiredAccuracy = GetLocatorAccuracy();

            actIndGettingLocation.IsRunning = true;
            actIndGettingLocation.IsVisible = true;
            btnGetLocation.IsEnabled = false;
            btnToggleLocationUpdates.IsEnabled = false;

            var position = await locator.GetPositionAsync(timeoutMilliseconds: 5000);
            latestPosition = position;

            UpdateUIWithNewPosition(position);
            geofenceManager.UpdateGeofences(position);

            actIndGettingLocation.IsRunning = false;
            actIndGettingLocation.IsVisible = false;
            btnGetLocation.IsEnabled = true;
            btnToggleLocationUpdates.IsEnabled = true;
        }

        private void UpdateUIWithNewPosition(Position position)
        {
            if (position != null)
            {
                //lblTimeStamp.Text = position.Timestamp.ToString();
                lblTimeStamp.Text = position.Timestamp.LocalDateTime.AddHours(1).ToString();
                lblLocation.Text = String.Concat(position.Latitude, ", ", position.Longitude);
                lblHeading.Text = String.Concat(position.Heading, " deg rel N");

                // In v4.0
                //var addresses = await locator.GetAddressesForPositionAsync(position);
                //var address = addresses.FirstOrDefault();
                //if (address == null)
                //{
                //    lblAddress.Text = "No address found.";
                //}
                //else
                //{
                //    lblAddress.Text = String.Concat(address.Throughfare, Environment.NewLine, address.Locality, ", ", address.Country);
                //}
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        StackLayout mainStack { get; set; }

        Label lblTimeStamp { get; } = new Label
        {
            Text = String.Empty,
            HorizontalTextAlignment = TextAlignment.Center,
            HorizontalOptions = LayoutOptions.FillAndExpand
        };

        Label lblLocation { get; } = new Label
        {
            Text = String.Empty,
            HorizontalTextAlignment = TextAlignment.Center,
            HorizontalOptions = LayoutOptions.FillAndExpand
        };

        Label lblHeading { get; } = new Label
        {
            Text = String.Empty,
            HorizontalTextAlignment = TextAlignment.Center,
            HorizontalOptions = LayoutOptions.FillAndExpand
        };

        Label lblAddress { get; } = new Label
        {
            Text = String.Empty,
            HorizontalTextAlignment = TextAlignment.Center,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            LineBreakMode = LineBreakMode.WordWrap
        };

        Button btnShowOnMap = new Button
        {
            Text = "Show On Map",
            HorizontalOptions = LayoutOptions.FillAndExpand
        };

        Button btnGetLocation = new Button
        {
            Text = "Get Location",
            HorizontalOptions = LayoutOptions.FillAndExpand
        };

        static int defaultUpdateInterval = 20;
        Entry entryUpdateInterval = new Entry
        {
            Text = defaultUpdateInterval.ToString(),
            //Placeholder = "Interval in Seconds",
            Keyboard = Keyboard.Numeric,
            HorizontalTextAlignment = TextAlignment.Center,
            //HorizontalOptions = LayoutOptions.Center
        };

        static int defaultMinDistance = 0;
        Entry entryMinimumDistance = new Entry
        {
            Text = defaultMinDistance.ToString(),
            //Placeholder = "Distance in Meters",
            Keyboard = Keyboard.Numeric,
            HorizontalTextAlignment = TextAlignment.Center
        };

        Button btnToggleLocationUpdates = new Button
        {
            Text = "Start Updates",
            HorizontalOptions = LayoutOptions.FillAndExpand
        };

        static int defaultDesiredAccuracy = 0;
        Entry entryDesiredAccuracy = new Entry
        {
            Text = defaultDesiredAccuracy.ToString(),
            //Placeholder = "Accuracy in Meters",
            Keyboard = Keyboard.Numeric,
            HorizontalTextAlignment = TextAlignment.Center,
            //HorizontalOptions = LayoutOptions.Center,
        };

        ActivityIndicator actIndGettingLocation = new ActivityIndicator
        {
            IsRunning = false,
            IsVisible = false
        };
    }
}
