   M S B A M L   `   `   `  ����B  >CityApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:CityManagementApp.ViewsCityManagementApp.Views  5   	   I)clr-namespace:CityManagementApp.ViewModelCityManagementApp.ViewModel  6=   0    *CityManagementApp.Views.EntityEditorWindow   5   	   W SPresentationCore, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7local%clr-namespace:CityManagementApp.Views     5   	   ?	viewmodel)clr-namespace:CityManagementApp.ViewModel     6=     � Title$  EntityEditorWindow��5   	   $	��400��6$   $	��600��61    � 	Resources 5	       �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius �� Triggers P� Property	 P� Value��%�  '��       �� 5   
   " ��6   .�5      ��  � �� �#3B59986+   �� 5       � ��
 �����6+   �� 5       � ��$
 Bold"�6+   �� 5       � ��$
 10,5q�6(   �� 5       � ��$ 5q�6'   �� 5       � ��
 ���� 6,   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� #��z��6!   $ 5��5   !   #��z��5   !   $��1q�5   !   ._�5      �� $��Center=�6/   $��Center=�5   /   5       5      P� 5       �@J�IsMouseOver$
	 True��6=   . �5   "   ��  � �� �#8B9DC36?   5      P� 5        � ��$
	 True��6;   . �5!   "   ��  � �� �#D8DFEA6?   5"      5$      5&      5'      5(      .��5)      � $��10q�6   
 � RowDefinitions
 5*   
   �� 5+      $��* �6   �� 5,      $
��Auto �6   .+�5/   
   �� $��0��6   $
��Auto=�6$   .��50      �� .+�51      �� ��6   �� *Fields+��52      �� 53      .R�54      �� $��5q�6)   .+�55   "   �� $
��Bold"�6B   ��6,   �� *Name+6B   �� 56   "   ��6*   �� *Value+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�.��6t   ��57   &   �� 58   *   " ��60   .�59   .   ��  � {�$
 True��6J    �� Triggers 5:   .   �� 5;   2    �� Binding 6>   �� *
IsEditable+ �� Value	 False6]   .a�5<   6   ��  � �� �#F0F0F06S   �� 5=   6    � {�$ False��6R   5>   3   5@   +   .��5A   5   5B   #   5C      5D      5F      5G      5H      �� 5J   
   $��1��6    �� Orientation$ 
Horizontal=�6"   $��Right=�6;   .+�5K      �� $��Сохранить��6   $��5q�6I   $��23��6T   $��76��6`   ��6)   �� *SaveCommand+6`   �� 5L      $��Отмена��6   $��5q�6H   $��32��6S   $��56��6_   ��6&   �� *CancelCommand+6_   5M      5N      5O      