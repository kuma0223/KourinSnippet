InputDialog["���O", "�o�^��", "����"]

$name = $$Input1
$day1 = $$Input2
$day2 = $$Input3

$str = ""
$str = $str & $name & "�l�A���������p�����������肪�Ƃ��������܂��B\n"
$str = $str & "���o�^����" & $day1 & "���1�J�����o�߂������܂����̂ŁA�A���P�[�g�̂����͂����肢���܂��B\n"
$str = $str & "���񓚂���������ꍇ��" & $day2 & "�܂łɕԐM���Ă���������ƍK���ł��B\n"
$str = $str & "��낵�����肢�������܂��B\n\n"
$str = $str & format["{0:yyyy�NMM��dd��}", date] & " �T�[�r�X���P�S��"
$str