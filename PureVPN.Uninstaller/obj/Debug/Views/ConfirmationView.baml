   M S B A M L   `   `   `  ????  Uninstaller0    *PureVPN.Uninstaller.Views.ConfirmationView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5        ? 	Resources  5	       ?? Source?? 5
   
   $9 3/Uninstaller;component/Resources/Styles/Styles.xaml??6   %    .??5      ? .+?5   
   ?? $??Center=?6   $??Center=?62   $??
0,-120,0,0q?6M   .+?5      ?? $
??Bold"?6   
???????$??25(?6=   $(??"PureVPN is about to be uninstalled??6K   $??Center=?6u   ?? 5      $6??0Click "Continue" to proceed or "Cancel" to exit.??6    ?? TextAlignment$ Center=?6d   $	??470??6{   
???????$
??Wrap=?6?   $??15(?6?   $??5,15,0,0q?6?   $
??Left=?6?   5      ?? 5   
    ?? Orientation$ 
Horizontal=?6   $??Right=?6.   $??Bottom=?6J   $??	0,0,25,25q?6e   .+?5      ?? $??Cancel??6      CurvedButton2#??[  6&   $??SemiBold"?6M   4  .System.Windows.Automation.AutomationProperties   AutomationId$ btnUninstallCancel??6?   ??6c   ?? *CancelAction+6?   ?? 5      $??Continue??6   $??15,0,0,0q?6(     CurvedButton#??[ 6:   $??SemiBold"?6`   $ btnUninstallContinue??6?   ??6v   ?? *ContinueAction+6?   5      5       ? Triggers 5      :? 5   
    :? RoutedEvent$ Loaded??6   .[?5      ?? .??5      ?? .??5      W?  ?? TargetProperty$ Opacity?6&    W? From$ 0X?6J   		 W? To$	 1X?6S   
 g? Duration$
 00:00:2D?6[   5       5!      5"      :? 5#   
   $ Unloaded??6   .[?5$      ?? .??5%      ?? .??5&      W? $ Opacity?6&   $ 1X?6J   $	 0X?6S   $
 0:0:0D?6[   5'      5(      5)      5+      