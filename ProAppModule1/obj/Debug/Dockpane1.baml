   M S B A M L   `   `   `  ����H  DProAppModule1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:ProAppModule1ProAppModule1  5      U QArcGIS.Desktop.Extensions, Version=11.4.0.0, Culture=neutral, PublicKeyToken=nullhJclr-namespace:ArcGIS.Desktop.Extensions;assembly=ArcGIS.Desktop.ExtensionsArcGIS.Desktop.Extensions 5      !    ProAppModule1.Dockpane1View   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      *uiclr-namespace:ProAppModule1     5      a
extensionsJclr-namespace:ArcGIS.Desktop.Extensions;assembly=ArcGIS.Desktop.Extensions     5        � 	Resources  5       �� MergedDictionaries<  6ArcGIS.Desktop.Extensions.DesignOnlyResourceDictionary   Source�� 5   
    5        5      $S Mpack://application:,,,/ArcGIS.Desktop.Framework;component\Themes\Default.xaml?�6:   %    5      .��5      �  J� RenderTransformOrigin$ 	0.3,0.487&�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   ]� $��0��6   ��. -  'System.Windows.Input.KeyboardNavigation   TabNavigation$ Local=�66   $��30��6_   .+�5      �� $��1��6   $��Center=�6A   $��Center=�6\   $��16(�5      $��Light"�6#   ��5   (   �� *	Heading+ � ToolTip 5      � 5       � Orientation$ Vertical=�6    $	��300��67   .+�5      �� $
��Wrap=�6=   ��6$   �� *	Heading+6=   5      5       5!      �� 5"   
   -   5         ��Name$ mapProjectItemLabel��5"      $��Map Project Item��6-   $
��Left=�6H   $��	10,10,0,0q�6c   $��1��6v   $	��Top=�6�   �� 5#   
   -   5        	 ��Name$	 mapProjectItemComboBox��5#      $
��Left=�63   $��	10,31,0,0q�6N   $��1��6a   $	��Top=�6n   $	��280��6�   ��6�   �� *MapProjectItemNames+�� 5$   
   -   5        $ 
layerLabel��5$      $��Layer��6$   $
��Left=�64   $��	10,58,0,0q�6O   $��1��6b   $	��Top=�6o   �� 5%   
   -   5        $	 layerComboBox��5%      $
��Left=�6*   $	��Top=�6E   $	��280��6]   $��	10,79,0,0q�6i   $��22��6|   $��1��6�   ��6�   �� *
LayerNames+�� 5&   
   -   5        $ neighborhoodLabel��5&      $��Neighborhood��6+   $
��Left=�6B   $��
10,106,0,0q�6]   $��1��6q   $	��Top=�6~   �� 5'   
   -   5        $	 neighborhoodComboBox��5'      $��10,127,10,0q�61   $	��Top=�6F   $��1��6^   ��6�   �� *NeighborhoodNames+5(      5)      