   M S B A M L   `   `   `  ����G  CCommunicator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:CommunicatorCommunicator  5      T PMahApps.Metro, Version=1.3.0.0, Culture=neutral, PublicKeyToken=f4fb5a3c4d1e5b4fV;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.MetroMahApps.Metro.Controls 5          Communicator.ChatWindow   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      ,localclr-namespace:Communicator     5      PControls;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro     5        � Title$  Skype��5      $	��400��5       � Icon$ Communicator.ico��5      $	��600��5	       � 
ResizeMode$ CanMinimize=�6#   (  "MahApps.Metro.Controls.MetroWindow   ShowMaxRestoreButton .   � 	Resources 5
      +   %Communicator.XamlToTextBlockConverter   xamlConverter��%   &	          5   
   6)   5      .��5      �  � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $	��120 �6    � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $��60 �6   .+�5   
   �� -   5        $��listOfMessagesLabel��5   -   $��0��6   $��0��6   $��Wiadomości��6H   $��Center=�6]   �� 5   
   -   5        $��messageScroll��5   F   $��0��6   $��0��6$   $��0 30 0 0q�64   .��5      �� -   5        $��listOfMessages��5      ��5      �� 5      .R�5      � $��2,5,0,0q�6    5      �� 5   "   $	��100 �63   �� 5   "   $��* �63    5       �� 5!   "   $��* �60   �� 5"   "   $��3 �60   .+�5$      �� $��0��6(   $��0��68   �� 5%   "   $
��Bold"�6F   $
��Wrap=�6u   ��6,   �� *
UserName+��6X   �� *Color+6u   5&      �� 5'      $��1��6(   $��0��68   �� 5(   "   
�������$
��10px(�6X   ��6,   �� *
DateTime+6j   �� 5)   "   �� 5*   "   ��61   �� *Text+ �� 	Converter# [  5+      �� 5,      $��1��6)   $��0��66   $��2��6F   .+�5-   "   �� 5.      5/      50      52      54      �� 55   
   -   5        $��listOfUsersLabel��55   -   $��0��6   $��1��6   $��Lista użytkowników��6E   $��Center=�6b   �� 56   
   $��0��6   $��1��6$   $��5 30 0 0q�64   .��57      �� -   5        $��listOfUsers��57      ��58      �� 59      .R�5:      � $��2,0,0,0q�6    5;      �� 5<   "   $��10 �63   �� 5=   "   $��* �63   .+�5?      �� $'��!http://hmt.pl/skin/img/profil.png��6$   6Q   �� 5@      $
��Wrap=�6`   $��1��6t   $��5 0 0 0q�6�   $
��10px(�6�   ��6(   �� *
UserName+��6C   �� *Color+6�   5A      5B      5D      5E      �� 5F   
   -   5         ��Name$
 send��5F      $��Wyślij��6   $��1��60   $
��Left=�6@   $��1��6[   $	��Top=�6h   $��75��6�   $��Center=�6�   $��	35,25,0,0q�6�     AccentedSquareButtonStyle#��[ 6�   6�   �� 5G   
   -   5        	 ��Name$	 message��5G   H   *  $MahApps.Metro.Controls.TextBoxHelper
   	Watermark$
 Twoja wiadomość...��6   $
��Left=�6Y   $��60��6t   $��1��6�    �� TextWrapping$
 Wrap=�6�   $	��Top=�6�   $	��470��6�   6�   �� 5H   
   	 �� X1$ 0��6   	 �� Y1$ 25��6   	 �� X2$	 600��6   	 �� Y2$ 25��6'   
���   �$��1��6>   $��0��6R   $��2��6b   $��0��6v   5I      5J      