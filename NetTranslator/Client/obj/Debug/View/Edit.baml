   M S B A M L   `   `   `  ����A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:Client.TypeConvertersClient.TypeConverters  5
   	   %clr-namespace:Client.VM	Client.VM  5   	       Client.ClientViewEdit   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5	   	   3cnv#clr-namespace:Client.TypeConverters     5
   	   &vmclr-namespace:Client.VM     5   	   -   5          J� 	AllowDrop  .  � AllowsTransparency .  J� OpacityMask
 ����� � WindowStyle$
 None=�5   	   
������  � Title$ The language Translator��5   	   $	��350��6)   $	��525��66    � DataContext 5        Client.VM.editVM  5   
    � 	Resources 5      /   )Client.TypeConverters.enumToBoolConverter   enumToBoolConverter��%   &	          5   
   6"   5      .��5      � .+�5   
   �� $��../images/book2.png��6   6/   �� 5      $��	Add word ��6   $
��Left=�6�   $	��Top=�6�   $��
35,117,0,0q�6�   $��97��6�   ��6)   �� *AddWordToVocabulary+��6Q   ��  �� ElementName$ translatedWord�� �� Path$
 Text�6�   �� 5   
   	 � ToolTip$<	 6Enter the word to be translated or added to dictionary��6\   $
��Left=�6�   $��23��6�   
 �� TextWrapping$

 Wrap=�6�   $	��Top=�6�   $	��195��6�   $��	32,84,0,0q�6�   ��6   �� $ TranslatedWord� �� UpdateSourceTrigger$ PropertyChanged=�6�   �� 5   
   $��Remove word��6   $
��Left=�6U   $	��Top=�6p   $��97��6�   $��
35,139,0,0q�6�   ��6'   �� *RemoveWordFromVocabulary+6�   �� 5   
   $��Update word��6   $
��Left=�6R   $	��Top=�6m   $��92��6�   $��
35,161,0,0q�6�   ��6'   �� *UpdateWordToVocabulary+6�   �� 5   
   $
��Left=�6   $	��Top=�6+   $��	35,53,0,0q�6C   $	��192��6V   �� 5   
   $
��Left=�6*   $	��Top=�6E   $��
246,53,0,0q�6]   $	��219��6q   ��6   �� *Item5+6q   �� 5   
   $��Get translation��6   $
��Left=�6d   $	��Top=�6   $��92��6�   $��
35,183,0,0q�6�   ��6+   �� $% GetTranslatedWordFromVocabulary�6�   �� 5   
   -   5        $��translations��5   �   $
��Left=�6�   $	��179��6�   $	��Top=�6�   $	��219��6�   $��246,139,0,0q�6�    �� SelectedItem 6   �� *SelectedTranslation+ �� Mode$ TwoWay=�$ PropertyChanged=���6q   �� $ Translations�6�   �� 5   
   -   5        $��	AddedWord��5   �   $V	 PEnter translation of the word specified in the left field and add it to the list��6[   $
��Left=�6�   $��23��6�   $

 Wrap=�6�   $	��Top=�6  $	��178��6  $��
287,79,0,0q�6&  ��6   �� *TranslationCurrent+$ PropertyChanged=�6&  �� 5   
   $��+��6   $	 Add to translations��6�   $
��Left=�6�   $	��Top=�6�   $��36��6�   $��
246,80,0,0q�6�   ��6   �� $ AddToTranslations���6H   �� $ 	AddedWord��$
 Text�6�   �� 5   
   $��Translation mode��6   $
��Left=�6+   $	��Top=�6F   $��	83,11,0,0q�6^   $	��117��6q   $��37��6}   	� 5    
    	� 	GroupName$ 	TransMode��6   $��from English to Russian��6�   $
��Left=�6�   $	��Top=�6�   $��
205,15,0,0q�6   d� 	IsChecked 6,   �� $ TranslationMode� �� 	Converter# [   �� ConverterParameter$ EngToRus��6  	� 5!   
   $ 	TransMode��6   $��from Russian to English��6�   $
��Left=�6�   $	��Top=�6�   $��
205,31,0,0q�6   6,   �� $ TranslationMode�# [  $ RusToEng��6  �� 5"   
   $
��Left=�6   $	��Top=�6.   $��	190,8,0,0q�6F    J� RenderTransformOrigin$ 0.5,0.5&�6Y   $	��173��6y   $��42��6�   z�5#      V� 5$      .~�5%      �� �� 5&      �� 5'       �� Angle$ -0.128Q�6&   T� 5(      5)      5+      �� 5,   
   $��-��6   $	 Remove from translations��6�   $
��Left=�6�   $	��Top=�6�   $��36��6�   $��246,112,0,0q�6�   ��6   �� *RemoveLastAddedTranslation+��6L   �� *Text+$ 	AddedWord��6�   �� 5-   
   $��	Clear all��6   $	 Clear translations list��6�   $
��Left=�6�   $	��Top=�6�   $��65��6�   $��400,117,0,0q�6�   $ 	1.389,0.5&�6�   ��6%   �� *RemoveAllTranslations+��6O   �� *Text+$ 	AddedWord��6�   �� 5.   
   $
��Left=�6*   $	��Top=�6E   $��	35,52,0,0q�6]   $	��192��6p   ��6   �� *Item4+6p   �� 5/   
   $��X��6   $
��Left=�66   $	��Top=�6Q   $��29��6i   $��	486,0,0,0q�6t   ��6   �� *Exit+6t   �� 50   
   $��_��6   $
��Left=�6@   $	��Top=�6[   $��29��6s   $��	457,0,0,0q�6~   ��6   �� *MinimizeWindow+6~   �� 51   
   
���  ��$
��Left=�6[   $	��Top=�6v   $��	8,269,0,0q�6�   ��6   �� *Status+$ 	LostFocus=�6�   53      54      