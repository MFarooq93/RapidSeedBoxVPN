   M S B A M L   `   `   `  ????  PureVPNInstaller+    %PureVPN.Installer.Views.InstallerView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5        ? 	Resources  5	       ?? MergedDictionaries ?? Source?? 5
   
    5      ?? 5      $> 8/PureVPNInstaller;component/Resources/Styles/Styles.xaml??6%   ?? 5      $I C/PureVPNInstaller;component/Resources/Styles/ProgressBarStyles.xaml??6%   %    5      .??5      ?  ? RowDefinitions 5   
   ?? 5      ?? 5      .+?5   
   ?? $??Center=?6   $	??490??60   $??0,65,0,0q?6<   .+?5      ?? $??20(?6:   
???????$??Center=?6[   $	??480??6x    ?? TextAlignment$ Center=?6?   ??6   ?? *InstallerMessage+6?   ?? 5      $?? ??6   $??15(?6    $??0,5,0,5q?6.   ???#b293d4$??Center=?6T   ? 5      $??18??6   $	??480??6&   
??????? ? Value 62   ?? *InstallProgress+6T   5      5       ? Triggers 5       :? 5!   
    :? RoutedEvent$ Loaded??6   .[?5"      ?? .??5#      ?? .??5$      W?  ?? TargetProperty$ Opacity?6&   	 W? From$	 0X?6J   	
 W? To$
 1X?6S    g? Duration$ 00:00:2D?6[   5%      5&      5'      :? 5(   
   $ Unloaded??6   .[?5)      ?? .??5*      ?? .??5+      W? $ Opacity?6&   $	 1X?6J   $
 0X?6S   $ 0:0:0D?6[   5,      5-      5.      51      