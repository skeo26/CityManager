   M S B A M L   `   `   `  ����B  >CityApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:CityManagementAppCityManagementApp  5   	   %    CityManagementApp.AddEditWindow   5   	   W SPresentationCore, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:CityManagementApp     5   	     � Title$  AddEditWindow��5   	   $	��450��6   $	��800��6,    � 	Resources 5	       �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius �� Triggers P� Property	 P� Value��%�  '��       �� 5   
   " ��6   .�5      ��  � �� �#3B59986+   �� 5       � ��
 �����6+   �� 5       � ��$
 Bold"�6+   �� 5       � ��$
 10,5q�6(   �� 5       � ��$ 5q�6'   �� 5       � ��
 ���� 6,   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� #��z��6!   $ 5��5   !   #��z��5   !   $��1q�5   !   ._�5      �� $��Center=�6/   $��Center=�5   /   5       5      P� 5       �@J�IsMouseOver$
	 True��6=   . �5   "   ��  � �� �#8B9DC36?   5      P� 5        � ��$
	 True��6;   . �5!   "   ��  � �� �#D8DFEA6?   5"      5$      5&      5'      5(      .��5)      � 
 � RowDefinitions
 5*   
   �� 5+      $
��Auto �6   �� 5,      $��* �6   �� 5-      $
��Auto �6   .+�51   
   �� $#��Добавить запись��6   $��20(�6+   $
��Bold"�69   $��Center=�52      $��10q�61   $��0��6=   �� 55   
   $��1��6   $��10q�6$   .��56      �� -   5        $��FieldsPanel��56      57      �� 5:   
    �� Orientation$ 
Horizontal=�6   $��Right=�6.   $��2��6J   $��10q�6W   .+�5;      �� -   5        $��
SaveButton��5;   )   $��Сохранить��6   $	��100��6;   $��5q�6G   6R   �� 5<      -   5        $��CancelButton��5<   &   $��Отмена��6   $	��100��6:   $��5q�6F   6Q   5=      5>      5?      