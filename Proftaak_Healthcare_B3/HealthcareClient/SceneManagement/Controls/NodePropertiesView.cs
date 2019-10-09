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
using System.Windows.Data;
using System.Windows.Media;
using UIControls.Fields;
using Xceed.Wpf.Toolkit;

namespace HealthcareClient.SceneManagement.Controls
{
    public delegate void SubmitNodeEventHandler(Node node);
    public delegate void CancelNodeEventHandler();

    public class NodePropertiesView : ContentControl
    {
        private Node node;

        private StackPanel nodePanel;
        private TextField nameField;
        private Vector3Field positionField;
        private NumberField scaleField;
        private Vector3Field rotationField;

        private WrapPanel modelSelection;
        private ModelSelectionField modelSelectionField;
        private CheckBox cullbackFacesCheckBox;

        private CheckBox hasTerrainCheckBox;
        private StackPanel terrainPropertiesPanel;
        private ImageSelectionField heightMapSelection;
        private WrapPanel terrainDimensionsPanel;
        private NumberField widthField;
        private NumberField depthField;
        private NumberField maxHeightField;
        private CheckBox smoothNormalsCheckBox;

        private CheckBox hasPanelCheckBox;
        private StackPanel panelPropertiesPanel;
        private WrapPanel sizePanel;
        private NumberField panelSizeXField;
        private NumberField panelSizeYField;
        private WrapPanel resolutionPanel;
        private NumberField panelResX;
        private NumberField panelResY;
        private CheckBox castShadowsCheckBox;
        private Label labelBackground;
        private ColorPicker backgroundPicker;

        private WrapPanel buttons;
        private StackPanel submitButton;
        private Label submitButtonContent;
        private StackPanel cancelButton;
        private Label cancelButtonContent;

        //private Style buttonStyle;

        private bool isUpdateProperties;

        private SubmitNodeEventHandler submitEventHandler;
        private CancelNodeEventHandler cancelEventHandler;
        private Session session;

        public NodePropertiesView(SubmitNodeEventHandler submitEventHandler, CancelNodeEventHandler cancelEventHandler, Session session, Node node)
        {
            this.nodePanel = new StackPanel();
            this.nameField = new TextField();
            this.positionField = new Vector3Field();
            this.scaleField = new NumberField();
            this.rotationField = new Vector3Field();

            this.modelSelection = new WrapPanel();
            this.modelSelectionField = new ModelSelectionField();
            this.cullbackFacesCheckBox = new CheckBox();

            this.hasTerrainCheckBox = new CheckBox();
            this.terrainPropertiesPanel = new StackPanel();
            this.heightMapSelection = new ImageSelectionField();
            this.terrainDimensionsPanel = new WrapPanel();
            this.widthField = new NumberField();
            this.depthField = new NumberField();
            this.maxHeightField = new NumberField();
            this.smoothNormalsCheckBox = new CheckBox();

            this.hasPanelCheckBox = new CheckBox();
            this.panelPropertiesPanel = new StackPanel();
            this.sizePanel = new WrapPanel();
            this.panelSizeXField = new NumberField();
            this.panelSizeYField = new NumberField();
            this.resolutionPanel = new WrapPanel();
            this.panelResX = new NumberField();
            this.panelResY = new NumberField();
            this.castShadowsCheckBox = new CheckBox();
            this.labelBackground = new Label();
            this.backgroundPicker = new ColorPicker();

            this.buttons = new WrapPanel();
            this.submitButton = new StackPanel();
            this.submitButtonContent = new Label();
            this.cancelButton = new StackPanel();
            this.cancelButtonContent = new Label();

            //this.buttonStyle = new Style();
            //this.buttonStyle.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30"))));
            //DataTrigger dataTrigger = new DataTrigger() { Binding = new Binding("IsMouseOver"), Value = true};
            //dataTrigger.Setters.Add(new Setter() { Property = Control.BackgroundProperty, Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF007ACC")) });
            //this.buttonStyle.Triggers.Add(dataTrigger);

            this.submitEventHandler = submitEventHandler;
            this.cancelEventHandler = cancelEventHandler;
            this.session = session;

            this.node = node;
            if(this.node != null)
                this.isUpdateProperties = true;
            else
                this.isUpdateProperties = false;


            CombineControls();
            SetupEvents();
            SetupControlStyles();

            if(this.node != null)
                SetupValues();
        }

        public NodePropertiesView(SubmitNodeEventHandler submitEventHandler, CancelNodeEventHandler cancelEventHandler, Session session)
            : this(submitEventHandler, cancelEventHandler, session, null) { }

        private void SetupValues()
        {
            this.nameField.Value = this.node.Name;
            this.positionField.SetVector3(new System.Numerics.Vector3(this.node.GetTransform().Position.X, this.node.GetTransform().Position.Y, this.node.GetTransform().Position.Z));
            this.scaleField.Value = this.node.GetTransform().Scale;
            this.rotationField.SetVector3(new System.Numerics.Vector3(this.node.GetTransform().Rotation.X, this.node.GetTransform().Rotation.Y, this.node.GetTransform().Rotation.Z));

            if(this.node.GetModel() != null)
            {
                this.modelSelectionField.SelectedValue = this.node.GetModel().Filename;
                this.cullbackFacesCheckBox.IsChecked = this.node.GetModel().CullbackFaces;
            }

            if(this.node.GetTerrain() != null)
            {
                this.heightMapSelection.SelectedValue = this.node.GetTerrain().HeightMapFilePath.Replace("/", "\\");
                this.hasTerrainCheckBox.IsChecked = true;
                this.smoothNormalsCheckBox.IsChecked = this.node.GetTerrain().SmoothNormals;

                this.widthField.Value = this.node.GetTerrain().Width;
                this.depthField.Value = this.node.GetTerrain().Depth;
                this.maxHeightField.Value = this.node.GetTerrain().MaxHeight;
            }

            if (this.node.GetPanel() != null)
            {
                this.hasPanelCheckBox.IsChecked = true;
                this.panelSizeXField.Value = this.node.GetPanel().Size.X;
                this.panelSizeYField.Value = this.node.GetPanel().Size.Y;
                this.panelResX.Value = this.node.GetPanel().Resolution.X;
                this.panelResY.Value = this.node.GetPanel().Resolution.Y;
                this.castShadowsCheckBox.IsChecked = this.node.GetPanel().CastShadow;

                this.backgroundPicker.SetValue(ColorPicker.SelectedColorProperty, Color.FromArgb((byte)this.node.GetPanel().Background.W,
                                                                                                    (byte)this.node.GetPanel().Background.X, 
                                                                                                    (byte)this.node.GetPanel().Background.Y, 
                                                                                                    (byte)this.node.GetPanel().Background.Z));
            }
        }

        private void CombineControls()
        {
            this.terrainDimensionsPanel.Children.Add(this.widthField);
            this.terrainDimensionsPanel.Children.Add(this.depthField);
            this.terrainDimensionsPanel.Children.Add(this.maxHeightField);
            this.terrainDimensionsPanel.Children.Add(this.smoothNormalsCheckBox);

            this.terrainPropertiesPanel.Children.Add(this.heightMapSelection);
            this.terrainPropertiesPanel.Children.Add(this.terrainDimensionsPanel);

            this.sizePanel.Children.Add(this.panelSizeXField);
            this.sizePanel.Children.Add(this.panelSizeYField);
            this.resolutionPanel.Children.Add(this.panelResX);
            this.resolutionPanel.Children.Add(this.panelResY);

            this.panelPropertiesPanel.Children.Add(this.sizePanel);
            this.panelPropertiesPanel.Children.Add(this.resolutionPanel);
            this.panelPropertiesPanel.Children.Add(this.castShadowsCheckBox);
            this.panelPropertiesPanel.Children.Add(this.labelBackground);
            this.panelPropertiesPanel.Children.Add(this.backgroundPicker);

            this.submitButton.Children.Add(this.submitButtonContent);
            this.cancelButton.Children.Add(this.cancelButtonContent);

            this.buttons.Children.Add(this.submitButton);
            this.buttons.Children.Add(this.cancelButton);

            this.modelSelection.Children.Add(this.modelSelectionField);
            this.modelSelection.Children.Add(this.cullbackFacesCheckBox);

            this.nodePanel.Children.Add(this.nameField);
            this.nodePanel.Children.Add(this.positionField);
            this.nodePanel.Children.Add(this.scaleField);
            this.nodePanel.Children.Add(this.rotationField);

            this.nodePanel.Children.Add(this.modelSelection);
            this.nodePanel.Children.Add(this.hasTerrainCheckBox);
            this.nodePanel.Children.Add(this.terrainPropertiesPanel);

            this.nodePanel.Children.Add(this.hasPanelCheckBox);
            this.nodePanel.Children.Add(this.panelPropertiesPanel);
            this.nodePanel.Children.Add(this.buttons);

            this.Content = this.nodePanel;
        }

        private void SetupEvents()
        {
            this.hasTerrainCheckBox.Checked += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                this.terrainPropertiesPanel.Visibility = Visibility.Visible;
            });
            this.hasTerrainCheckBox.Unchecked += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                this.terrainPropertiesPanel.Visibility = Visibility.Collapsed;
            });
            this.hasPanelCheckBox.Checked += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                this.panelPropertiesPanel.Visibility = Visibility.Visible;
            });
            this.hasPanelCheckBox.Unchecked += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                this.panelPropertiesPanel.Visibility = Visibility.Collapsed;
            });

            this.submitButton.MouseDown += SubmitButton_MouseDown;
            this.cancelButton.MouseDown += CancelButton_MouseDown;

            //isfModelDiffuse.Dictionary = ResourcesDictionary.TextureResources;
            //isfModelDiffuse.Value = ResourcesDictionary.TextureResources.Keys;

            this.modelSelectionField.Dictionary = ResourcesDictionary.ModelResources;
            this.modelSelectionField.Value = ResourcesDictionary.ModelResources.Keys;
            this.heightMapSelection.Value = ResourcesDictionary.HeightMaps;
        }

        private void SetupControlStyles()
        {
            if (this.isUpdateProperties)
            {
                this.nameField.IsEnabled = false;

                if (this.node.GetModel() == null)
                {
                    this.modelSelection.Visibility = Visibility.Collapsed;
                }

                if (this.node.GetTerrain() != null)
                {
                    this.terrainPropertiesPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    this.hasTerrainCheckBox.Visibility = Visibility.Collapsed;
                    this.terrainPropertiesPanel.Visibility = Visibility.Collapsed;
                }

                if (this.node.GetPanel() != null)
                {
                    this.panelPropertiesPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    this.hasPanelCheckBox.Visibility = Visibility.Collapsed;
                    this.panelPropertiesPanel.Visibility = Visibility.Collapsed;
                }

                this.submitButtonContent.Content = "Update Node";
                this.cancelButtonContent.Content = "Hide";
            }
            else
            {
                this.terrainPropertiesPanel.Visibility = Visibility.Collapsed;
                this.panelPropertiesPanel.Visibility = Visibility.Collapsed;
                this.submitButtonContent.Content = "Add Node";
                this.cancelButtonContent.Content = "Cancel";
            }

            this.nameField.Header = "Name:";
            this.positionField.Header = "Position:";
            this.scaleField.Header = "Scale:";
            this.rotationField.Header = "Rotation:";
            this.modelSelectionField.Header = "Model:";
            this.cullbackFacesCheckBox.Content = "Cullbackfaces";

            this.hasTerrainCheckBox.Content = "Has terrain";
            this.heightMapSelection.Header = "Heightmap:";
            this.widthField.Header = "Width:";
            this.depthField.Header = "Depth:";
            this.maxHeightField.Header = "Max height:";
            this.smoothNormalsCheckBox.Content = "Smooth normals";

            this.hasPanelCheckBox.Content = "Has panel";
            this.panelSizeXField.Header = "Size X:";
            this.panelSizeYField.Header = "Sixe Y:";
            this.panelResX.Header = "Res X:";
            this.panelResY.Header = "Res Y:";
            this.castShadowsCheckBox.Content = "Cast shadows";
            this.labelBackground.Content = "Background:";

            this.nameField.ApplyDarkTheme();
            this.positionField.ApplyDarkTheme();
            this.scaleField.ApplyDarkTheme();
            this.rotationField.ApplyDarkTheme();
            this.modelSelectionField.ApplyDarkTheme();

            this.heightMapSelection.ApplyDarkTheme();
            this.widthField.ApplyDarkTheme();
            this.depthField.ApplyDarkTheme();
            this.maxHeightField.ApplyDarkTheme();

            this.panelSizeXField.ApplyDarkTheme();
            this.panelSizeYField.ApplyDarkTheme();
            this.panelResX.ApplyDarkTheme();
            this.panelResY.ApplyDarkTheme();
            this.labelBackground.Foreground = Brushes.White;
            this.backgroundPicker.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30"));

            this.cullbackFacesCheckBox.Foreground = Brushes.White;
            this.hasTerrainCheckBox.Foreground = Brushes.White;
            this.smoothNormalsCheckBox.Foreground = Brushes.White;
            this.hasPanelCheckBox.Foreground = Brushes.White;
            this.castShadowsCheckBox.Foreground = Brushes.White;

            //this.button.Style = this.buttonStyle;
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

            this.modelSelectionField.MaxViewportSize = 150;
            this.heightMapSelection.MaxImageSize = 150;

            this.cullbackFacesCheckBox.Margin = new Thickness(0, 5, 0, 5);
            this.hasTerrainCheckBox.Margin = new Thickness(0, 5, 0, 5);
            this.smoothNormalsCheckBox.Margin = new Thickness(0, 5, 0, 5);
            this.hasPanelCheckBox.Margin = new Thickness(0, 5, 0, 5);
            this.castShadowsCheckBox.Margin = new Thickness(0, 5, 0, 5);

            this.cullbackFacesCheckBox.VerticalAlignment = VerticalAlignment.Bottom;
            this.smoothNormalsCheckBox.VerticalAlignment = VerticalAlignment.Bottom;

        }

        private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as StackPanel).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30"));
        }

        private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as StackPanel).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF007ACC"));
        }

        private void SubmitButton_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(!String.IsNullOrEmpty(this.nameField.Value))
            {
                Vector3 position = new Vector3(this.positionField.X, this.positionField.Y, this.positionField.Z);
                Vector3 rotation = new Vector3(this.rotationField.X, this.rotationField.Y, this.rotationField.Z);

                if(!this.isUpdateProperties)
                    this.node = new Node(this.nameField.Value, this.session);
                this.node.SetTransform(new HealthcareServer.Vr.World.Components.Transform(position, this.scaleField.Value, rotation));

                if (this.modelSelectionField.SelectedValue != null && !String.IsNullOrEmpty(this.modelSelectionField.SelectedValue.ToString()))
                    this.node.SetModel(new Model(this.modelSelectionField.SelectedValue.ToString(), (this.cullbackFacesCheckBox.IsChecked == true) ? true : false));

                if (this.hasTerrainCheckBox.IsChecked == true)
                    this.node.SetTerrain(new Terrain((int)this.widthField.Value, (int)this.depthField.Value, this.maxHeightField.Value, this.heightMapSelection.SelectedValue.ToString(), true, this.session));

                if (this.hasPanelCheckBox.IsChecked == true)
                    this.node.SetPanel(new HealthcareServer.Vr.World.Components.Panel(new Vector2(this.panelSizeXField.Value, this.panelSizeYField.Value),
                                                                                        new Vector2(this.panelResX.Value, this.panelResY.Value),
                                                                                        new Vector4(this.backgroundPicker.SelectedColor.Value.R, this.backgroundPicker.SelectedColor.Value.G, this.backgroundPicker.SelectedColor.Value.B, this.backgroundPicker.SelectedColor.Value.A),
                                                                                        (this.castShadowsCheckBox.IsChecked == true) ? true : false, this.session));

                this.submitEventHandler(this.node);
            }
            else
            {
                System.Windows.MessageBox.Show("Name cannot be empty!");
            }
        }

        private void CancelButton_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.cancelEventHandler();
        }
    }
}
