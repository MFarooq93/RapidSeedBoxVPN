   M S B A M L   `   `   `  ????  PureVPNInstaller1    +PureVPN.Installer.Views.AdditionalTasksView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5        ? 	Resources  5	       ?? Source?? 5
   
   $> 8/PureVPNInstaller;component/Resources/Styles/Styles.xaml??6   %    .??5      ? .+?5   
   ?? $??Center=?6   $	??Top=?62   $??0,0,0,0q?6J   .+?5      ?? $
??Bold"?6   
???????$??25(?6=   $+??%Where do you want to Install PureVPN???6K   $	??270??6x   $
??Wrap=?6?    ?? TextAlignment$ Center=?6?   $??Center=?6?   ? 5      $??0,15,0,0q?6   $??60??6%   $	??600??61    ? ColumnDefinitions 5      ?? 5      $??* ?6'   ?? 5      $	??110 ?6'   .+?5      ??    TextBoxStyle#??[  6i   $??17(?6?   
 ? Tag$" Enter installation path here??6?   $??Center=?6?   $??0q?6?   ??6   ?? *EnteredPath+ ?? Mode$ TwoWay=? ?? UpdateSourceTrigger$ PropertyChanged=?6?   ?? 5        SimpleButton#??[ 6   $??1??6?   $??0q?6O   $??Browse??6c   ??6t   ?? *FileDialogAction+5      ? 5      $??
-10,50,0,0q?6   $	??480??6'   $
??Left=?66    5      ?? 5      ?? 5      5!      5#      ?? 5%   
    ?? Orientation$ 
Horizontal=?6   $??Right=?6.   $??Bottom=?6J   $??	0,0,25,25q?6e   .+?5&      ?? $
??Back??6     CurvedButton2#??[ 6$   $??SemiBold"?6K   4  .System.Windows.Automation.AutomationProperties   AutomationId$ btnLocationBack??6?   ??6a   ?? *GoBackAction+6?   ?? 5'      $??Install??6>   $??15,0,0,0q?6P     CurvedButton#??[ 6b   $??SemiBold"?6?   $ btnLocationInstall??6?   {?6   ?? *IsEnteredPathValid+??6?   ?? *GoNextAction+6?   5(      5*      	 ? Triggers	 5,      :? 5-   
   
 :? RoutedEvent$
 Loaded??6   .[?5.      ?? .??5/      ?? .??50      W?  ?? TargetProperty$ Opacity?6&    W? From$ 0X?6J   	 W? To$ 1X?6S    g? Duration$ 00:00:2D?6[   51      52      53      :? 54   
   $
 Unloaded??6   .[?55      ?? .??56      ?? .??57      W? $ Opacity?6&   $ 1X?6J   $ 0X?6S   $ 0:0:0D?6[   58      59      5:      5=      