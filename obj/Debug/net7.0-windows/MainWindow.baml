   M S B A M L   `   `   `  ����B  >CityApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:CityManagementAppCityManagementApp  5   	   I)clr-namespace:CityManagementApp.ViewModelCityManagementApp.ViewModel  67   "    CityManagementApp.MainWindow   5   	   W SPresentationCore, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:CityManagementApp     5   	   ?	viewmodel)clr-namespace:CityManagementApp.ViewModel     67     � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)    � 	Resources 5	       �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius �� Triggers P� Property	 P� Value&   System.Windows.Controls.DataGrid=  7System.Windows.Controls.Primitives.DataGridColumnHeader��%  '��       '  �    �� 5   
   " ��6   .�5      ��  � �� �#3B59986+   �� 5       � ��
 �����6+   �� 5       � ��$
 Bold"�6+   �� 5       � ��$
 10,5q�6(   �� 5       � ��$ 5q�6'   �� 5       � ��
 ���� 6,   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� #��z��6!   $ 5��5   !   #��z��5   !   $��1q�5   !   ._�5      �� $��Center=�6/   $��Center=�5   /   5       5      P� 5       �@J�IsMouseOver$
	 True��6=   . �5   "   ��  � �� �#8B9DC36?   5      P� 5        � ��$
	 True��6;   . �5!   "   ��  � �� �#D8DFEA6?   5"      5$      5&      5'      �� 5*   
   "  6   .�5+      ��  � ��
 �����6+   �� 5,       � �� �#D3D3D36,   �� 5-       � ��$ 1q�60   �� 5.       �@ RowBackground �#F9F9F96.   �� 5/        �@ AlternatingRowBackground �#E9E9E969   �� 50       �@ ColumnHeaderStyle 51      �� 52      "  6   .�53      ��  � �� �#3B599867   �� 54       � ��
 �����67   �� 55       � ��$
 Bold"�67   �� 56       � ��$ 5q�64   �� 57       � ��$ Center=�6G   58      5:      5;      5<      .��5>      � 
 � RowDefinitions
 5?   
   �� 5@      $
��Auto �6   �� 5A      $��* �6   �� 5B      $
��Auto �6   .+�5E   
   �� $��10q�5G      ��5E      �� *
TableNames+ �� SelectedItem 5F      �� *SelectedTable+5G        5I   
   -   5        $��dataGrid��5I         AutoGenerateColumns$
 True��5L      $��1��5M      $��10q�6   ��5J      �� *Items+ 5K      �� *SelectedItem+5M      �� 5O   
   $��2��6    �� Orientation$ 
Horizontal=�6"   $��Center=�6;   $��10q�6X   .+�5P      �� $��Добавить��6   $	��100��6G   ��6(   �� *
AddCommand+6G   �� 5Q      $��Изменить��6   $	��100��6H   ��6(   �� *EditCommand+{�6T   �� *IsItemSelected+�� 5R      $��Удалить��6   $	��100��6I   ��6'   �� *DeleteCommand+{�6U   �� *IsItemSelected+5S      5T      5U      