﻿using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms; 

namespace MIOStopsVisualization
{
    public partial class StartWindow : Form
    {
        public static MIOApp app;

        int index;
        public static GMapMarker testBus;

        GMapOverlay zona0;
        GMapOverlay zona1;
        GMapOverlay zona2;
        GMapOverlay zona3;
        GMapOverlay zona4;
        GMapOverlay zona5;
        GMapOverlay zona6;
        GMapOverlay zona7;
        GMapOverlay zona8;

        GMapPolygon poligonoZ0;
        GMapPolygon poligonoZ1;
        GMapPolygon poligonoZ2;
        GMapPolygon poligonoZ3;
        GMapPolygon poligonoZ4;
        GMapPolygon poligonoZ5;
        GMapPolygon poligonoZ6;
        GMapPolygon poligonoZ7;
        GMapPolygon poligonoZ8;

        public StartWindow()
        {
            testBus = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(
                        new GMap.NET.PointLatLng(3.4041000, -76.5467650),
                            new Bitmap("images/MIO_BUS.png"));

            GMapOverlay markers = new GMapOverlay("markers");

            markers.Markers.Add(testBus);

            InitializeComponent();

            stopMap.Overlays.Add(markers);

            app = new MIOApp();
            index = 0;
            app.setTestBus("3.4041000", "-76.5467650", "383");

            saveCoordinates();

            //ReadFile();
            lbOpt.BackColor = Color.Transparent;
            var options = new List<String>();
            options.Add("OPCIONES");
            options.Add("ESTACIONES");
            options.Add("PARADAS");
            options.Add("ZONAS");
            this.optionComBox.DataSource = options;

            this.optionComBox.DropDownStyle = ComboBoxStyle.DropDownList;

        }

        public void saveCoordinates()
        {
            try
            {
                string line = "";
                StreamReader sr = new StreamReader("data/BDTEXTO.txt");
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Split(',')[3].Equals(app.getTestBus().getBusId()))
                    {
                        double newLat = app.adjustCoordinates(line.Split(',')[0]);
                        double newLon = app.adjustCoordinates(line.Split(',')[1]);

                        app.getTestBus().getCoordinates().Add(new KeyValuePair<double, double>(newLat, newLon));
                    }
                }
            } 
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public MIOApp getApp()
        {
            return app;
        }

        public void ReadFile()
        {
            Console.WriteLine("Entro");
            try
            {
                string line = "";
                StreamReader sr = new StreamReader("data/Stops.txt");
                //var watch = System.Diagnostics.Stopwatch.StartNew();
                //watch.Stop();
                //var elapsedMs = watch.ElapsedMilliseconds;
                //var timef = elapsedMs * 0.001;
                int i = 1;
                while ((line = sr.ReadLine()) != null)
                {
                    String[] individual = line.Split(',');

                    double lat = app.adjustCoordinates(individual[1]);
                    double lon = app.adjustCoordinates(individual[2]);

                    Stop st = new Stop(individual[0], lat, lon);
                    app.getStreetStop().Add(st);

                    Console.WriteLine("Hasta aqui va bien (Paradas) : " + i);
                    i++;
                }
                i = 1;
                MessageBox.Show("" + app.getStreetStop().Count);
                line = "";
                StreamReader sr2 = new StreamReader("data/Stations.txt");
                while ((line = sr2.ReadLine()) != null)
                {
                    String[] individual = line.Split(',');

                    double lat = app.adjustCoordinates(individual[1]);
                    double lon = app.adjustCoordinates(individual[2]);

                    Stop st = new Stop(individual[0], lat, lon);
                    app.getStationStop().Add(st);

                    Console.WriteLine("Hasta aqui va bien (Estaciones): " + i);
                    i++;
                }
                MessageBox.Show("" + app.getStationStop().Count);
                app.saveElements();
            }
            catch (Exception e)
            { 
                MessageBox.Show(e.Message);
            }
        }


        private void StopMap_Load(object sender, EventArgs e)
        {
            stopMap.DragButton = MouseButtons.Left;
            stopMap.MapProvider = GMapProviders.GoogleMap;
            double latitud = 3.4372201;
            double longitud = -76.5224991;
            stopMap.Position = new PointLatLng(latitud, longitud);
            stopMap.MinZoom = 1;
            stopMap.MaxZoom = 100;
            stopMap.Zoom = 13;
        }


        private void drawStops()
        {
            stopMap.Overlays.Clear();

            GMapOverlay markers = new GMapOverlay("markers");
            foreach (var aux in getApp().getStreetStop())
            {
                stopsMarker(aux.DecLati, aux.DecLong);
            }
            stopMap.Overlays.Add(markers);
            stopMap.Zoom = stopMap.Zoom + 1;
            stopMap.Zoom = stopMap.Zoom - 1;
            
        }

        private void stopsMarker(double lat, double lng)
        {
            PointLatLng point = new PointLatLng(lat, lng);
            GMapMarker theMarker = new GMarkerGoogle(point, GMarkerGoogleType.red_dot);

            GMapOverlay markers = new GMapOverlay("markers");

            markers.Markers.Add(theMarker);
            stopMap.Overlays.Add(markers);
        }

        public void drawStations()
        {
            stopMap.Overlays.Clear();

            foreach (var aux in getApp().getStationStop())
            {
                stationsMarker(aux.DecLati, aux.DecLong);
            }
            stopMap.Zoom = stopMap.Zoom + 1;
            stopMap.Zoom = stopMap.Zoom - 1;
           
        }

        private void stationsMarker(double lat, double lng)
        {
            PointLatLng point = new PointLatLng(lat, lng);
            GMapMarker theMarker = new GMarkerGoogle(point, GMarkerGoogleType.blue_dot);

            GMapOverlay markers = new GMapOverlay("markers");

            markers.Markers.Add(theMarker);
            stopMap.Overlays.Add(markers);
        }

        public void clearZone(GMapPolygon zone, GMapOverlay zz)
        {
            foreach (GMapMarker actual in stopMap.Overlays.OfType<GMapMarker>())
            {
                if (zone.IsInside(actual.Position))
                {
                    zz.Markers.Remove(actual);
                }
            }
            stopMap.Zoom = stopMap.Zoom + 1;
            stopMap.Zoom = stopMap.Zoom - 1;
        }

        public void drawInZone(GMapOverlay zone, GMapPolygon poly, int op)
        {
            if (op==1)
            {
                foreach (var actual in getApp().getStationStop())
                {
                    var p = new PointLatLng(actual.DecLati, actual.DecLong);
                    if (poly.IsInside(p))
                    {
                        GMapMarker theMarker = new GMarkerGoogle(p, GMarkerGoogleType.blue_dot);
                        poly.Overlay.Markers.Add(theMarker);
                    }
                }
            }
            else if (op==-1)
            {
                foreach (var actual in getApp().getStreetStop())
                {
                    var p = new PointLatLng(actual.DecLati, actual.DecLong);
                    if (poly.IsInside(p))
                    {
                        GMapMarker theMarker = new GMarkerGoogle(p, GMarkerGoogleType.red_dot);
                        poly.Overlay.Markers.Add(theMarker);
                    }
                }
            }
            else if(op==0)
            {
                foreach (var actual in getApp().getStationStop())
                {
                    var p = new PointLatLng(actual.DecLati, actual.DecLong);
                    if (poly.IsInside(p))
                    {
                        GMapMarker theMarker = new GMarkerGoogle(p, GMarkerGoogleType.blue_dot);
                        poly.Overlay.Markers.Add(theMarker);
                    }
                }
                foreach (var actual in getApp().getStreetStop())
                {
                    var p = new PointLatLng(actual.DecLati, actual.DecLong);
                    if (poly.IsInside(p))
                    {
                        GMapMarker theMarker = new GMarkerGoogle(p, GMarkerGoogleType.red_dot);
                        poly.Overlay.Markers.Add(theMarker);
                    }
                }
            }
            else
            {
                MessageBox.Show("POR FAVOR ESCOGA SI DESEA VER LAS ESTACIONES O LAS PARADAS");
                if (cbCentro.Checked == true)
                {
                    cbCentro.Checked = false;
                }
                else if(cbValleDeLili.Checked == true)
                {
                    cbValleDeLili.Checked = false;
                }
                else if (cbMenga.Checked == true)
                {
                    cbMenga.Checked = false;
                }
                else if (cbCalima.Checked == true)
                {
                    cbCalima.Checked = false;
                }
                else if(cbAguablanca.Checked == true)
                {
                    cbAguablanca.Checked = false;
                }
                else if (cbCiudadCordoba.Checked == true)
                {
                    cbCiudadCordoba.Checked = false;
                }
                else if (cbGuadalupe.Checked == true)
                {
                    cbGuadalupe.Checked = false;
                }
                else if(cbCañaveralejo.Checked == true)
                {
                    cbCañaveralejo.Checked = false;
                }
                else if(cbPrado.Checked == true)
                {
                    cbPrado.Checked = false;
                }
            }
            stopMap.Zoom = stopMap.Zoom + 1;
            stopMap.Zoom = stopMap.Zoom - 1;
        }

        public int verification2()
        {
            if (cbStation.Checked && cbStops.Checked)
            {
                return 0;
            }
            else  if (cbStation.Checked)
            {
                return 1;
            }
            else if (cbStops.Checked)
            {
                return -1;
            }
            else
            {
                return 999;
            }
        }

        public void drawZones()
        {
            stopMap.Overlays.Clear();

            stopMap.Zoom = stopMap.Zoom + 1;
            stopMap.Zoom = stopMap.Zoom - 1;

            cbCentro.Visible = true;
            cbMenga.Visible = true;
            cbCalima.Visible = true;
            cbAguablanca.Visible = true;
            cbCiudadCordoba.Visible = true;
            cbCañaveralejo.Visible = true;
            cbValleDeLili.Visible = true;
            cbPrado.Visible = true;
            cbGuadalupe.Visible = true;
            cbStation.Visible = true;
            cbStops.Visible = true;
            lbChoose.Visible = true;
            lbOpt.Location = new Point(12,271);
            optionComBox.Location = new Point(40,300);
            btnSatelite.Location = new Point(25, 501);
            btnRelieve.Location = new Point(106, 501);
            btnNormal.Location = new Point(64, 530);
            LbZoom.Location = new Point(37, 556);
            tbZoom.Location = new Point(40, 572);
        }

        public void drawZone0()
        {
            zona0 = new GMapOverlay("Zona 0");
            List<PointLatLng> puntosZ0 = new List<PointLatLng>();
            puntosZ0.Add(new PointLatLng(3.458952, -76.513447));
            puntosZ0.Add(new PointLatLng(3.463029, -76.521014));
            puntosZ0.Add(new PointLatLng(3.460478, -76.527052));
            puntosZ0.Add(new PointLatLng(3.450851, -76.537823));
            puntosZ0.Add(new PointLatLng(3.451531, -76.541646));
            puntosZ0.Add(new PointLatLng(3.455851, -76.542208));
            puntosZ0.Add(new PointLatLng(3.458972, -76.553556));
            puntosZ0.Add(new PointLatLng(3.458552, -76.582726));
            puntosZ0.Add(new PointLatLng(3.448433, -76.562660));
            puntosZ0.Add(new PointLatLng(3.452630, -76.549432));
            puntosZ0.Add(new PointLatLng(3.441510, -76.536748));
            puntosZ0.Add(new PointLatLng(3.428423, -76.533186));
            puntosZ0.Add(new PointLatLng(3.434846, -76.523287));
            puntosZ0.Add(new PointLatLng(3.443270, -76.518067));

            poligonoZ0 = new GMapPolygon(puntosZ0, "CENTRO");
            zona0.Polygons.Add(poligonoZ0);
            stopMap.Overlays.Add(zona0);
            stopMap.Zoom = stopMap.Zoom + 1;
            stopMap.Zoom = stopMap.Zoom - 1;

            int op = verification2();

            drawInZone(zona0, poligonoZ0, op);

        }

        public void drawZone1()
        {
            zona1 = new GMapOverlay("Zona 1");
            List<PointLatLng> puntosZ1 = new List<PointLatLng>();
            puntosZ1.Add(new PointLatLng(3.387889, -76.513426));
            puntosZ1.Add(new PointLatLng(3.388507, -76.519616));
            puntosZ1.Add(new PointLatLng(3.387518, -76.524073));
            puntosZ1.Add(new PointLatLng(3.392462, -76.524631));
            puntosZ1.Add(new PointLatLng(3.392956, -76.532493));
            puntosZ1.Add(new PointLatLng(3.387827, -76.532647));
            puntosZ1.Add(new PointLatLng(3.384706, -76.560319));
            puntosZ1.Add(new PointLatLng(3.380813, -76.558586));
            puntosZ1.Add(new PointLatLng(3.380381, -76.562300));
            puntosZ1.Add(new PointLatLng(3.373645, -76.559390));
            puntosZ1.Add(new PointLatLng(3.370925, -76.553571));
            puntosZ1.Add(new PointLatLng(3.371060, -76.543718));
            puntosZ1.Add(new PointLatLng(3.367526, -76.541765));
            puntosZ1.Add(new PointLatLng(3.361442, -76.542623));
            puntosZ1.Add(new PointLatLng(3.353045, -76.549232));
            puntosZ1.Add(new PointLatLng(3.342849, -76.551550));
            puntosZ1.Add(new PointLatLng(3.315172, -76.546142));
            puntosZ1.Add(new PointLatLng(3.314658, -76.523140));
            puntosZ1.Add(new PointLatLng(3.314572, -76.522882));
            puntosZ1.Add(new PointLatLng(3.325626, -76.522367));
            puntosZ1.Add(new PointLatLng(3.348247, -76.522968));
            puntosZ1.Add(new PointLatLng(3.348932, -76.506574));
            puntosZ1.Add(new PointLatLng(3.355787, -76.509492));
            puntosZ1.Add(new PointLatLng(3.354073, -76.516273));
            puntosZ1.Add(new PointLatLng(3.364698, -76.516359));
            puntosZ1.Add(new PointLatLng(3.366069, -76.508377));
            puntosZ1.Add(new PointLatLng(3.371381, -76.509578));
            puntosZ1.Add(new PointLatLng(3.382049, -76.507089));

            poligonoZ1 = new GMapPolygon(puntosZ1, "VALLE DE LILI");
            zona1.Polygons.Add(poligonoZ1);
            stopMap.Overlays.Add(zona1);
            stopMap.Zoom = stopMap.Zoom + 1;
            stopMap.Zoom = stopMap.Zoom - 1;

            int op = verification2();

            drawInZone(zona1, poligonoZ1, op);
        }

        public void drawZone2()
        {
            zona2 = new GMapOverlay("Zona 2");
            List<PointLatLng> puntosZ2 = new List<PointLatLng>();
            puntosZ2.Add(new PointLatLng(3.463029, -76.521014));
            puntosZ2.Add(new PointLatLng(3.460478, -76.527052));
            puntosZ2.Add(new PointLatLng(3.450851, -76.537823));
            puntosZ2.Add(new PointLatLng(3.451531, -76.541646));
            puntosZ2.Add(new PointLatLng(3.455851, -76.542208));
            puntosZ2.Add(new PointLatLng(3.454776, -76.540491));
            puntosZ2.Add(new PointLatLng(3.472167, -76.531307));
            puntosZ2.Add(new PointLatLng(3.475791, -76.532884));
            puntosZ2.Add(new PointLatLng(3.478440, -76.529107));
            puntosZ2.Add(new PointLatLng(3.481190, -76.529311));
            puntosZ2.Add(new PointLatLng(3.488169, -76.537783));
            puntosZ2.Add(new PointLatLng(3.489748, -76.538549));
            puntosZ2.Add(new PointLatLng(3.494435, -76.535436));
            puntosZ2.Add(new PointLatLng(3.489850, -76.528852));
            puntosZ2.Add(new PointLatLng(3.496918, -76.526449));
            puntosZ2.Add(new PointLatLng(3.498546, -76.522458));
            puntosZ2.Add(new PointLatLng(3.506085, -76.521685));
            puntosZ2.Add(new PointLatLng(3.516365, -76.519454));
            puntosZ2.Add(new PointLatLng(3.519449, -76.533659));
            puntosZ2.Add(new PointLatLng(3.527416, -76.526020));
            puntosZ2.Add(new PointLatLng(3.520434, -76.519239));
            puntosZ2.Add(new PointLatLng(3.521805, -76.517651));
            puntosZ2.Add(new PointLatLng(3.525615, -76.506765));
            puntosZ2.Add(new PointLatLng(3.523958, -76.495361));
            puntosZ2.Add(new PointLatLng(3.505948, -76.500053));
            puntosZ2.Add(new PointLatLng(3.505516, -76.492691));
            puntosZ2.Add(new PointLatLng(3.499343, -76.492160));

            poligonoZ2 = new GMapPolygon(puntosZ2, "MENGA");
            zona2.Polygons.Add(poligonoZ2);
            stopMap.Overlays.Add(zona2);
            stopMap.Zoom = stopMap.Zoom + 1;
            stopMap.Zoom = stopMap.Zoom - 1;

            int op = verification2();

            drawInZone(zona2, poligonoZ2, op);
        }

        public void drawZone3()
        {
            zona3 = new GMapOverlay("Zona 3");
            List<PointLatLng> puntosZ3 = new List<PointLatLng>();
            puntosZ3.Add(new PointLatLng(3.499343, -76.492160));
            puntosZ3.Add(new PointLatLng(3.497010, -76.485022));
            puntosZ3.Add(new PointLatLng(3.482987, -76.482092));
            puntosZ3.Add(new PointLatLng(3.480321, -76.479688));
            puntosZ3.Add(new PointLatLng(3.477062, -76.479344));
            puntosZ3.Add(new PointLatLng(3.474059, -76.476589));
            puntosZ3.Add(new PointLatLng(3.468334, -76.475987));
            puntosZ3.Add(new PointLatLng(3.458952, -76.513447));
            puntosZ3.Add(new PointLatLng(3.463029, -76.521014));

            poligonoZ3 = new GMapPolygon(puntosZ3, "CALIMA");
            zona3.Polygons.Add(poligonoZ3);
            stopMap.Overlays.Add(zona3);
            stopMap.Zoom = stopMap.Zoom + 1;
            stopMap.Zoom = stopMap.Zoom - 1;

            int op = verification2();

            drawInZone(zona3, poligonoZ3, op);
        }

        public void drawZone4()
        {
            zona4 = new GMapOverlay("Zona4");
            List<PointLatLng> puntosZ4 = new List<PointLatLng>();
            puntosZ4.Add(new PointLatLng(3.443270, -76.518067));
            puntosZ4.Add(new PointLatLng(3.458952, -76.513447));
            puntosZ4.Add(new PointLatLng(3.468334, -76.475987));
            puntosZ4.Add(new PointLatLng(3.465410, -76.474958));
            puntosZ4.Add(new PointLatLng(3.460690, -76.478052));
            puntosZ4.Add(new PointLatLng(3.440685, -76.472893));
            puntosZ4.Add(new PointLatLng(3.441297, -76.466824));
            puntosZ4.Add(new PointLatLng(3.436129, -76.463077));
            puntosZ4.Add(new PointLatLng(3.399183, -76.466336));
            puntosZ4.Add(new PointLatLng(3.400404, -76.476322));
            puntosZ4.Add(new PointLatLng(3.413793, -76.482086));
            puntosZ4.Add(new PointLatLng(3.418084, -76.486381));
            puntosZ4.Add(new PointLatLng(3.428568, -76.483722));
            puntosZ4.Add(new PointLatLng(3.446679, -76.484064));
            puntosZ4.Add(new PointLatLng(3.443159, -76.493519));

            poligonoZ4 = new GMapPolygon(puntosZ4, "AGUABLANCA");
            zona4.Polygons.Add(poligonoZ4);
            stopMap.Overlays.Add(zona4);
            stopMap.Zoom = stopMap.Zoom + 1;
            stopMap.Zoom = stopMap.Zoom - 1;

            int op = verification2();

            drawInZone(zona4, poligonoZ4, op);
        }

        public void drawZone5()
        {
            zona5 = new GMapOverlay("Zonas 5");
            List<PointLatLng> puntosZ5 = new List<PointLatLng>();
            puntosZ5.Add(new PointLatLng(3.443827, -76.491165));
            puntosZ5.Add(new PointLatLng(3.428302, -76.498642));
            puntosZ5.Add(new PointLatLng(3.407200, -76.521070));
            puntosZ5.Add(new PointLatLng(3.404841, -76.518878));
            puntosZ5.Add(new PointLatLng(3.402954, -76.520984));
            puntosZ5.Add(new PointLatLng(3.403125, -76.522445));
            puntosZ5.Add(new PointLatLng(3.400466, -76.523132));
            puntosZ5.Add(new PointLatLng(3.394890, -76.523605));
            puntosZ5.Add(new PointLatLng(3.394333, -76.519308));
            puntosZ5.Add(new PointLatLng(3.390859, -76.519738));
            puntosZ5.Add(new PointLatLng(3.390215, -76.516300));
            puntosZ5.Add(new PointLatLng(3.391288, -76.516000));
            puntosZ5.Add(new PointLatLng(3.393732, -76.513164));
            puntosZ5.Add(new PointLatLng(3.388929, -76.509598));
            puntosZ5.Add(new PointLatLng(3.392801, -76.504657));
            puntosZ5.Add(new PointLatLng(3.402838, -76.499415));
            puntosZ5.Add(new PointLatLng(3.414504, -76.483088));
            puntosZ5.Add(new PointLatLng(3.418084, -76.486381));
            puntosZ5.Add(new PointLatLng(3.428568, -76.483722));
            puntosZ5.Add(new PointLatLng(3.446679, -76.484064));

            poligonoZ5 = new GMapPolygon(puntosZ5, "CIUDAD CORDOBA");
            zona5.Polygons.Add(poligonoZ5);
            stopMap.Overlays.Add(zona5);
            stopMap.Zoom = stopMap.Zoom + 1;
            stopMap.Zoom = stopMap.Zoom - 1;

            int op = verification2();

            drawInZone(zona5, poligonoZ5, op);
        }

        public void drawZone6()
        {
            zona6 = new GMapOverlay("Zona 6");
            List<PointLatLng> puntosZ6 = new List<PointLatLng>();
            puntosZ6.Add(new PointLatLng(3.419485, -76.508024));
            puntosZ6.Add(new PointLatLng(3.419999, -76.508604));
            puntosZ6.Add(new PointLatLng(3.419432, -76.521462));
            puntosZ6.Add(new PointLatLng(3.434846, -76.523287));
            puntosZ6.Add(new PointLatLng(3.428423, -76.533186));
            puntosZ6.Add(new PointLatLng(3.398191, -76.538025));
            puntosZ6.Add(new PointLatLng(3.398277, -76.542537));
            puntosZ6.Add(new PointLatLng(3.386310, -76.544481));
            puntosZ6.Add(new PointLatLng(3.387827, -76.532647));
            puntosZ6.Add(new PointLatLng(3.392956, -76.532493));
            puntosZ6.Add(new PointLatLng(3.392462, -76.524631));
            puntosZ6.Add(new PointLatLng(3.387518, -76.524073));
            puntosZ6.Add(new PointLatLng(3.389252, -76.519946));
            puntosZ6.Add(new PointLatLng(3.389971, -76.520956));
            puntosZ6.Add(new PointLatLng(3.389209, -76.522588));
            puntosZ6.Add(new PointLatLng(3.392104, -76.523738));
            puntosZ6.Add(new PointLatLng(3.394890, -76.523605));
            puntosZ6.Add(new PointLatLng(3.400466, -76.523132));
            puntosZ6.Add(new PointLatLng(3.403125, -76.522445));
            puntosZ6.Add(new PointLatLng(3.402954, -76.520984));
            puntosZ6.Add(new PointLatLng(3.404841, -76.518878));
            puntosZ6.Add(new PointLatLng(3.407200, -76.521070));

            poligonoZ6 = new GMapPolygon(puntosZ6, "GUADALUPE");
            zona6.Polygons.Add(poligonoZ6);
            stopMap.Overlays.Add(zona6);
            stopMap.Zoom = stopMap.Zoom + 1;
            stopMap.Zoom = stopMap.Zoom - 1;

            int op = verification2();

            drawInZone(zona6, poligonoZ6, op);
        }

        public void drawZone7()
        {
            zona7 = new GMapOverlay("Zona 7");
            List<PointLatLng> puntosZ7 = new List<PointLatLng>();
            puntosZ7.Add(new PointLatLng(3.428423, -76.533186));
            puntosZ7.Add(new PointLatLng(3.441510, -76.536748));
            puntosZ7.Add(new PointLatLng(3.452630, -76.549432));
            puntosZ7.Add(new PointLatLng(3.448433, -76.562660));
            puntosZ7.Add(new PointLatLng(3.435942, -76.557605));
            puntosZ7.Add(new PointLatLng(3.427738, -76.571950));
            puntosZ7.Add(new PointLatLng(3.417393, -76.567787));
            puntosZ7.Add(new PointLatLng(3.411738, -76.574267));
            puntosZ7.Add(new PointLatLng(3.407067, -76.578130));
            puntosZ7.Add(new PointLatLng(3.402269, -76.571864));
            puntosZ7.Add(new PointLatLng(3.391773, -76.572207));
            puntosZ7.Add(new PointLatLng(3.384706, -76.560319));
            puntosZ7.Add(new PointLatLng(3.386310, -76.544481));
            puntosZ7.Add(new PointLatLng(3.398277, -76.542537));
            puntosZ7.Add(new PointLatLng(3.398191, -76.538025));

            poligonoZ7 = new GMapPolygon(puntosZ7, "CAÑAVERALEJO");
            zona7.Polygons.Add(poligonoZ7);
            stopMap.Overlays.Add(zona7);
            stopMap.Zoom = stopMap.Zoom + 1;
            stopMap.Zoom = stopMap.Zoom - 1;

            int op = verification2();

            drawInZone(zona7, poligonoZ7, op);
        }

        public void drawZone8()
        {
            zona8 = new GMapOverlay("Zona 8");
            List<PointLatLng> puntosZ8 = new List<PointLatLng>();
            puntosZ8.Add(new PointLatLng(3.443827, -76.491165));
            puntosZ8.Add(new PointLatLng(3.428302, -76.498642));
            puntosZ8.Add(new PointLatLng(3.419485, -76.508024));
            puntosZ8.Add(new PointLatLng(3.419999, -76.508604));
            puntosZ8.Add(new PointLatLng(3.419432, -76.521462));
            puntosZ8.Add(new PointLatLng(3.434846, -76.523287));
            puntosZ8.Add(new PointLatLng(3.443270, -76.518067));
            puntosZ8.Add(new PointLatLng(3.443159, -76.493519));

            poligonoZ8 = new GMapPolygon(puntosZ8, "PRADO");
            zona8.Polygons.Add(poligonoZ8);
            stopMap.Overlays.Add(zona8);
            stopMap.Zoom = stopMap.Zoom + 1;
            stopMap.Zoom = stopMap.Zoom - 1;

            int op = verification2();

            drawInZone(zona8, poligonoZ8, op);
        }

        private void OptionComBox_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            verification();
        }

        private void verification()
        {
            int ind = optionComBox.SelectedIndex;

            if (ind == 1)
            {
                drawStations();
            }
            else if (ind == 2)
            {
                drawStops();
            }
            else if (ind == 3)
            {
                drawZones();
            }
            else
            {

            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void btnSatelite_Click(object sender, EventArgs e)
        {
            stopMap.MapProvider = GMapProviders.GoogleChinaSatelliteMap;
        }

        private void btnNormal_Click(object sender, EventArgs e)
        {
            stopMap.MapProvider = GMapProviders.GoogleMap;
        }

        private void btnRelieve_Click(object sender, EventArgs e)
        {
            stopMap.MapProvider = GMapProviders.GoogleTerrainMap;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tbZoom.Value = Convert.ToInt32(stopMap.Zoom);
        }

        private void tbZoom_ValueChanged(object sender, EventArgs e)
        {
            stopMap.Zoom = tbZoom.Value;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
                               
            if (index < app.getTestBus().getCoordinates().Count)
            {
                double newLat = app.getTestBus().getCoordinates()[index].Key;
                double newLon = app.getTestBus().getCoordinates()[index].Value;
                testBus.Position = new PointLatLng(newLat, newLon);
                index++;
            }
            else
            {
                timer2.Stop();
            }
            
        }

        private void cbValleDeLili_CheckedChanged(object sender, EventArgs e)
        {
            if (cbValleDeLili.Checked == true)
            {
                drawZone1();
            }
            else
            {
                zona1.Clear();
            }
        }

        private void cbMenga_CheckedChanged(object sender, EventArgs e)
        {
            if (cbMenga.Checked == true)
            {
                drawZone2();
            }
            else
            {
                zona2.Clear();
            }
        }

        private void cbCañaveralejo_CheckedChanged(object sender, EventArgs e)
        {
            if (cbCañaveralejo.Checked == true)
            {
                drawZone7();
            }
            else
            {
                zona7.Clear();
            }
        }

        private void cbCentro_CheckedChanged(object sender, EventArgs e)
        {
            if (cbCentro.Checked == true)
            {
                drawZone0();
            }
            else
            {
                zona0.Clear();
            }
        }

        private void cbGuadalupe_CheckedChanged(object sender, EventArgs e)
        {
            if (cbGuadalupe.Checked == true)
            {
                drawZone6();
            }
            else
            {
                zona6.Clear();
            }
        }

        private void cbPrado_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPrado.Checked == true)
            {
                drawZone8();
            }
            else
            {
                zona8.Clear();
            }
        }

        private void cbAguablanca_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAguablanca.Checked == true)
            {
                drawZone4();
            }
            else
            {
                zona4.Clear();
            }
        }

        private void cbCalima_CheckedChanged(object sender, EventArgs e)
        {
            if (cbCalima.Checked == true)
            {
                drawZone3();
            }
            else
            {
                zona3.Clear();
            }
        }

        private void cbCiudadCordoba_CheckedChanged(object sender, EventArgs e)
        {
            if (cbCiudadCordoba.Checked == true)
            {
                drawZone5();
            }
            else
            {
                clearZone(poligonoZ5, zona5);
                zona5.Clear();
            }
        }

        private void StartWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void cbStation_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbStops_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
