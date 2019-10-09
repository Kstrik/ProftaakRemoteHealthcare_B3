using HealthcareClient.Resources;
using HealthcareServer.Vr;
using HealthcareServer.Vr.VectorMath;
using HealthcareServer.Vr.World;
using HealthcareServer.Vr.World.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UIControls.Fields;

namespace HealthcareClient.SceneManagement.Controls
{
    public delegate void SubmitRouteEventHandler(Route route);
    public delegate void CancelRouteEventHandler();

    public class RoutePropertiesView : ContentControl
    {
        private Route route;

        private StackPanel routePanel;
        private Label vectorsLabel;
        private StackPanel vectorPanel;

        private WrapPanel vectorButtons;
        private StackPanel addVectorButton;
        private Label addVectorButtonContent;
        private StackPanel removeVectorButton;
        private Label removeVectorButtonContent;

        private CheckBox hasRoadCheckBox;
        private StackPanel roadPropertiesPanel;
        private ImageSelectionField diffuseTexturesSelection;
        private ImageSelectionField normalTexturesSelection;
        private ImageSelectionField specularTexturesSelection;
        private NumberField heightOffsetField;

        private WrapPanel buttons;
        private StackPanel submitButton;
        private Label submitButtonContent;
        private StackPanel cancelButton;
        private Label cancelButtonContent;

        private bool isUpdateProperties;

        private SubmitRouteEventHandler submitEventHandler;
        private CancelRouteEventHandler cancelEventHandler;
        private Session session;

        public RoutePropertiesView(SubmitRouteEventHandler submitEventHandler, CancelRouteEventHandler cancelEventHandler, Session session, Route route)
        {
            this.routePanel = new StackPanel();
            this.vectorsLabel = new Label();
            this.vectorPanel = new StackPanel();

            this.vectorButtons = new WrapPanel();
            this.addVectorButton = new StackPanel();
            this.addVectorButtonContent = new Label();
            this.removeVectorButton = new StackPanel();
            this.removeVectorButtonContent = new Label();

            this.hasRoadCheckBox = new CheckBox();
            this.roadPropertiesPanel = new StackPanel();
            this.diffuseTexturesSelection = new ImageSelectionField();
            this.normalTexturesSelection = new ImageSelectionField();
            this.specularTexturesSelection = new ImageSelectionField();
            this.heightOffsetField = new NumberField();

            this.buttons = new WrapPanel();
            this.submitButton = new StackPanel();
            this.submitButtonContent = new Label();
            this.cancelButton = new StackPanel();
            this.cancelButtonContent = new Label();

            this.submitEventHandler = submitEventHandler;
            this.cancelEventHandler = cancelEventHandler;
            this.session = session;

            this.route = route;
            if (this.route != null)
                this.isUpdateProperties = true;
            else
                this.isUpdateProperties = false;

            CombineControls();
            SetupEvents();
            SetupControlStyles();

            if (this.route != null)
                SetupValues();
        }

        public RoutePropertiesView(SubmitRouteEventHandler submitEventHandler, CancelRouteEventHandler cancelEventHandler, Session session)
            : this(submitEventHandler, cancelEventHandler, session, null) { }

        private void SetupValues()
        {
            foreach(Route.RouteNode routeNode in this.route.GetRouteNodes())
            {
                Vector3Field position = new Vector3Field();
                Vector3Field direction = new Vector3Field();
                position.Header = "Position:";
                direction.Header = "Direction:";
                position.ApplyDarkTheme();
                direction.ApplyDarkTheme();

                position.X = routeNode.Position.X;
                position.Y = routeNode.Position.Y;
                position.Z = routeNode.Position.Z;
                direction.X = routeNode.Direction.X;
                direction.Y = routeNode.Direction.Y;
                direction.Z = routeNode.Direction.Z;

                WrapPanel vectorPanel = new WrapPanel();
                vectorPanel.Children.Add(position);
                vectorPanel.Children.Add(direction);

                this.vectorPanel.Children.Add(vectorPanel);
            }

            if(this.route.GetRouteNodes().Count > 2)
            {
                this.removeVectorButton.Visibility = Visibility.Visible;
            }

            if (this.route.Road != null)
            {
                this.diffuseTexturesSelection.SelectedValue = this.route.Road.Diffuse.Replace("/", "\\");
                this.normalTexturesSelection.SelectedValue = this.route.Road.Normal.Replace("/", "\\");
                this.specularTexturesSelection.SelectedValue = this.route.Road.Specular.Replace("/", "\\");
                this.heightOffsetField.Value = this.route.Road.HeightOffset;
                this.hasRoadCheckBox.IsChecked = true;
            }
        }

        private void CombineControls()
        {
            if(!this.isUpdateProperties)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector3Field position = new Vector3Field();
                    Vector3Field direction = new Vector3Field();
                    position.Header = "Position:";
                    direction.Header = "Direction:";
                    position.ApplyDarkTheme();
                    direction.ApplyDarkTheme();

                    WrapPanel vectorPanel = new WrapPanel();
                    vectorPanel.Children.Add(position);
                    vectorPanel.Children.Add(direction);

                    this.vectorPanel.Children.Add(vectorPanel);
                }
            }

            this.roadPropertiesPanel.Children.Add(this.diffuseTexturesSelection);
            this.roadPropertiesPanel.Children.Add(this.normalTexturesSelection);
            this.roadPropertiesPanel.Children.Add(this.specularTexturesSelection);
            this.roadPropertiesPanel.Children.Add(this.heightOffsetField);

            this.addVectorButton.Children.Add(this.addVectorButtonContent);
            this.removeVectorButton.Children.Add(this.removeVectorButtonContent);

            this.vectorButtons.Children.Add(this.addVectorButton);
            this.vectorButtons.Children.Add(this.removeVectorButton);

            this.submitButton.Children.Add(this.submitButtonContent);
            this.cancelButton.Children.Add(this.cancelButtonContent);

            this.buttons.Children.Add(this.submitButton);
            this.buttons.Children.Add(this.cancelButton);

            this.routePanel.Children.Add(this.vectorsLabel);
            this.routePanel.Children.Add(this.vectorPanel);
            this.routePanel.Children.Add(this.vectorButtons);
            this.routePanel.Children.Add(this.hasRoadCheckBox);
            this.routePanel.Children.Add(this.roadPropertiesPanel);
            this.routePanel.Children.Add(this.buttons);

            this.Content = this.routePanel;
        }

        private void SetupEvents()
        {
            this.hasRoadCheckBox.Checked += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                this.roadPropertiesPanel.Visibility = Visibility.Visible;
            });
            this.hasRoadCheckBox.Unchecked += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                this.roadPropertiesPanel.Visibility = Visibility.Collapsed;
            });

            this.addVectorButton.MouseDown += AddVector_MouseDown;
            this.removeVectorButton.MouseDown += RemoveVector_MouseDown;

            this.submitButton.MouseDown += SubmitButton_MouseDown;
            this.cancelButton.MouseDown += CancelButton_MouseDown;

            this.diffuseTexturesSelection.Dictionary = ResourcesDictionary.DiffuseTextureResources;
            this.diffuseTexturesSelection.Value = ResourcesDictionary.DiffuseTextureResources.Keys;
            this.normalTexturesSelection.Dictionary = ResourcesDictionary.NormalTextureResources;
            this.normalTexturesSelection.Value = ResourcesDictionary.NormalTextureResources.Keys;
            this.specularTexturesSelection.Dictionary = ResourcesDictionary.SpecularTextureResources;
            this.specularTexturesSelection.Value = ResourcesDictionary.SpecularTextureResources.Keys;
        }

        private void SetupControlStyles()
        {
            if (this.isUpdateProperties)
            {
                if (this.route.Road != null)
                {
                    this.roadPropertiesPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    this.hasRoadCheckBox.Visibility = Visibility.Collapsed;
                    this.roadPropertiesPanel.Visibility = Visibility.Collapsed;
                }

                this.submitButtonContent.Content = "Update Route";
                this.cancelButtonContent.Content = "Hide";
            }
            else
            {
                this.roadPropertiesPanel.Visibility = Visibility.Collapsed;
                this.submitButtonContent.Content = "Add Route";
                this.cancelButtonContent.Content = "Cancel";
            }

            this.vectorsLabel.Content = "Vectors:";

            this.addVectorButtonContent.Content = "Add Vector";
            this.removeVectorButtonContent.Content = "Remove Vector";
            this.removeVectorButton.Visibility = Visibility.Collapsed;

            this.hasRoadCheckBox.Content = "Has road";
            this.diffuseTexturesSelection.Header = "Diffuse:";
            this.normalTexturesSelection.Header = "Normal:";
            this.specularTexturesSelection.Header = "Specular:";
            this.heightOffsetField.Header = "Heightoffset:";

            this.diffuseTexturesSelection.ApplyDarkTheme();
            this.normalTexturesSelection.ApplyDarkTheme();
            this.specularTexturesSelection.ApplyDarkTheme();
            this.heightOffsetField.ApplyDarkTheme();

            this.addVectorButton.Width = 100;
            this.addVectorButton.Height = 25;
            this.addVectorButton.Margin = new Thickness(5, 5, 5, 5);
            this.addVectorButton.MouseEnter += Button_MouseEnter;
            this.addVectorButton.MouseLeave += Button_MouseLeave;
            this.addVectorButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30"));
            this.addVectorButtonContent.Foreground = Brushes.White;

            this.removeVectorButton.Width = 100;
            this.removeVectorButton.Height = 25;
            this.removeVectorButton.Margin = new Thickness(0, 5, 5, 5);
            this.removeVectorButton.MouseEnter += Button_MouseEnter;
            this.removeVectorButton.MouseLeave += Button_MouseLeave;
            this.removeVectorButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30"));
            this.removeVectorButtonContent.Foreground = Brushes.White;

            this.submitButton.Width = 100;
            this.submitButton.Height = 25;
            this.submitButton.Margin = new Thickness(5, 5, 5, 5);
            this.submitButton.MouseEnter += Button_MouseEnter;
            this.submitButton.MouseLeave += Button_MouseLeave;
            this.submitButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30"));
            this.submitButtonContent.Foreground = Brushes.White;

            this.cancelButton.Width = 100;
            this.cancelButton.Height = 25;
            this.cancelButton.Margin = new Thickness(0, 5, 5, 5);
            this.cancelButton.MouseEnter += Button_MouseEnter;
            this.cancelButton.MouseLeave += Button_MouseLeave;
            this.cancelButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30"));
            this.cancelButtonContent.Foreground = Brushes.White;

            this.vectorsLabel.Foreground = Brushes.White;
            this.hasRoadCheckBox.Foreground = Brushes.White;

            this.diffuseTexturesSelection.MaxImageSize = 150;
            this.normalTexturesSelection.MaxImageSize = 150;
            this.specularTexturesSelection.MaxImageSize = 150;

            this.hasRoadCheckBox.Margin = new Thickness(0, 5, 0, 5);
        }

        private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as StackPanel).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30"));
        }

        private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as StackPanel).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF007ACC"));
        }

        private void AddVector_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Vector3Field position = new Vector3Field();
            Vector3Field direction = new Vector3Field();
            position.Header = "Position:";
            direction.Header = "Direction:";
            position.ApplyDarkTheme();
            direction.ApplyDarkTheme();

            WrapPanel vectorPanel = new WrapPanel();
            vectorPanel.Children.Add(position);
            vectorPanel.Children.Add(direction);

            this.vectorPanel.Children.Add(vectorPanel);

            if (this.vectorPanel.Children.Count > 2)
            {
                this.removeVectorButton.Visibility = Visibility.Visible;
            }
        }

        private void RemoveVector_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.vectorPanel.Children.RemoveAt(this.vectorPanel.Children.Count - 1);

            if (this.vectorPanel.Children.Count == 2)
            {
                this.removeVectorButton.Visibility = Visibility.Collapsed;
            }
        }

        private void SubmitButton_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!this.isUpdateProperties)
            {
                this.route = new Route(this.session);
            }

            this.route.GetRouteNodes().Clear();
            foreach (WrapPanel control in this.vectorPanel.Children)
            {
                Vector3Field positionField = control.Children[0] as Vector3Field;
                Vector3Field directionField = control.Children[1] as Vector3Field;

                Vector3 position = new Vector3(positionField.X, positionField.Y, positionField.Z);
                Vector3 direction = new Vector3(directionField.X, directionField.Y, directionField.Z);

                this.route.AddRouteNode(new Route.RouteNode(position, direction));
            }

            if (this.hasRoadCheckBox.IsChecked == true)
            {
                string diffuse = "";
                string normal = "";
                string specular = "";
                float heightOffset = 0.0f;

                if (this.diffuseTexturesSelection.SelectedValue != null && !String.IsNullOrEmpty(this.diffuseTexturesSelection.SelectedValue.ToString()))
                    diffuse = this.diffuseTexturesSelection.SelectedValue.ToString().Replace("/", "\\");

                if (this.normalTexturesSelection.SelectedValue != null && !String.IsNullOrEmpty(this.normalTexturesSelection.SelectedValue.ToString()))
                    normal = this.normalTexturesSelection.SelectedValue.ToString().Replace("/", "\\");

                if (this.specularTexturesSelection.SelectedValue != null && !String.IsNullOrEmpty(this.specularTexturesSelection.SelectedValue.ToString()))
                    specular = this.specularTexturesSelection.SelectedValue.ToString().Replace("/", "\\");

                heightOffset = this.heightOffsetField.Value;

                if (this.route.Road == null)
                {
                    this.route.SetRoad(new Road(diffuse, normal, specular, heightOffset, this.route, this.session));
                }
                else
                {
                    this.route.Road.Diffuse = diffuse;
                    this.route.Road.Normal = normal;
                    this.route.Road.Specular = specular;
                    this.route.Road.HeightOffset = heightOffset;
                }
            }

            this.submitEventHandler(this.route);
        }

        private void CancelButton_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.cancelEventHandler();
        }
    }
}
