   M S B A M L   `   `   `  ����E  ASyncromote, Version=0.2.0.0, Culture=neutral, PublicKeyToken=nulle7clr-namespace:Project_valkyrie__Name_subject_to_chnage_)Project_valkyrie__Name_subject_to_chnage_  5   	   :    4Project_valkyrie__Name_subject_to_chnage_.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NNHotkey.Wpf, Version=2.0.0.0, Culture=neutral, PublicKeyToken=a76ffb086700b11cF 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Ilocal7clr-namespace:Project_valkyrie__Name_subject_to_chnage_     5   	   D @ModernWpf, Version=0.9.4.0, Culture=neutral, PublicKeyToken=nullM IModernWpf.Controls, Version=0.9.4.0, Culture=neutral, PublicKeyToken=null,ui!http://schemas.modernwpf.com/2019   5   	     � Title$  Project Valkyrie��5   	   $	��400��5	   	   $	��600��5
   	   0  *ModernWpf.Controls.Primitives.WindowHelper   UseModernWindowStyle .   ModernWpf.ThemeManager   RequestedTheme$
 Dark=�5   	    � WindowStartupLocation$ CenterScreen=�5   	   ��5      �� 5   
    �� ImageSource 5      �� 5       �� 	UriSource$" img/mainpageshiinamayuri.jpg?�6   5      .��5      � .+�5   
   �� 
���   � J� Opacity$	 0.4Q�5      $��	0,-32,0,0q�5      '  !ModernWpf.Controls.NavigationView  5   
   -   5          Name$ NavView��5         IsBackButtonVisible$ Visible=�5      	   IsBackEnabled	 .  
   
IsPaneOpen
 .    PaneDisplayMode$ LeftCompact=�5!         OpenPaneLength$	 105Q�5"       K� ZIndex$ 1��5#         	MenuItems 5'      +  %ModernWpf.Controls.NavigationViewItem  5(      -   5          Name$ HomeBox��5*   (      Icon-  'ModernWpf.Controls.IconElementConverter$
 Home 5(   (   $
��Home��5)   (   5-        5.      -   5        $ ChatBox��50   (   $ Comment 5.   (   $
��Chat��5/   (   50   (     51      $ Phone 6(   $��About��52   (   58      � 5:   
   -   5         �Name$
 Home��5:      .+�5;      �� ���#404040$��1q�5<      ._�5=      � $	��400��6   $	��600��5>       � RowDefinitions 5@      �� 5A      $	��428 �6(   .+�5E      � $��20,0,0,0q�6   .+�5F      �� -   5         ��Name$ 
messageTxt��5F   $   
�������$
��Left=�5H   $   $��32��5I   $   $��10,0,0,0q�5J   $   $
��Wrap=�5K   $   $	��Top=�5L   $   $	��567��5M   $    �� TextAlignment$ Center=�5N   $   $��16(�5O   $   �� 5P      -   5        $��connectbutton��5P   !   $��Connect��5R   !   
�������$��15(�5T   !   $��SemiBold"�5U   !   $��44,173,0,217q�5V   !    �� 	IsDefault . $��38��5X   !   $	��145��5Y   !   5[      �� 5\      -   5        $��startButton��5]   !   $��Start��5\   !   
�������$��15(�5_   !   $��SemiBold"�5`   !   $��44,222,0,168q�5a   !    . $��38��5d   !   $	��145��5e   !   5g      �� 5i      -   5        $ ipTextBlock��5i   $   
�������$
��Left=�5k   $   $ 1��5l   $   $��17��5m   $   $��
54,127,0,0q�5n   $   $
��Wrap=�5o   $   $	��Top=�5p   $   $��17��5q   $   $��14(�5r   $   $��IP��5s   $   �� 5u      -	   5         ��Name$ 	ipTextBox��5u   "   $
��Left=�5v   "   $��27��5w   "   $��
44,120,0,0q�5x   "    �� TextWrapping$
 Wrap=�5y   "   $	��Top=�5z   "   $	��145��5{   "   $��16(�5|   "   $��0q�5~   "   5      �� 5�      -
   5        $��About us��5�      
�������$��15(�5�      $��SemiBold"�5�      $��44,314,0,75q�5�       . $��39��5�      $	��125��5�      $y�Hidden=�5�      5�      �� 5�      -   5        $��messagetextBox��5�      $y�Hidden=�5�      $
��Left=�5�      $��38��5�      $��342,302,0,0q�5�      $
 Wrap=�5�      $	��Top=�5�      $	��179��5�      $��18(�5�      
�������$	 0.7Q�5�      $��0q�5�      �� 5�      -   5        $y�Hidden=�5�      $
��Send��5�      
�������$��15(�5�      $��SemiBold"�5�      $��521,294,23,75q�5�       .  J� RenderTransformOrigin$ 2.509,0.653&�5�      ��5�      �� 5�       �� 
TargetType" ��6$   .�5�   "   ��  �� Property � �� �� Value �#50606b5�   )   �� 5�   "    �@�Cursor$
 Hand��5�   )    �� Triggers 5�   "   P� 5�   &    P� Property �@J�IsMouseOver P� Value . . �5�   *   ��  � �� �#3e331e5�   1   5�   '   5�      ��5�      �� 5�       �� 
TargetType" ��6.   .R�5�   "   �� $��45��6)   $��45��5�   )     �� CornerRadius$  20��5�   )   $��0q�5�   )   ���#737373#��z��5�   )   $ 1.733,0.533&�5�   )   ._�5�   &   �� $��Center=�67   $��Center=�5�   7   5�   #   5�      5�      �� 5�      $��Visible=�6#   $	��213��5�   #   $��342,76,23,139q�5�   #   .��5�      �� -   5        $��messagetextBlock��5�   %   
�������$	 0.7Q�5�   %   $y�Hidden=�5�   %   
���   �$
��Left=�5�   %   $	��213��5�   %   $
��Wrap=�5�   %   $	��Top=�5�   %   $	��226��5�   %   $��15(�5�   %   5�      5�      5�      5�      5�      5�      