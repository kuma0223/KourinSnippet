InputDialog["���O", "�o�^��", "����"]

$name = $$Input1
$day1 = $$Input2
$day2 = $$Input3
$today = format["{0:yyyy�NMM��dd��}", date]

$str = $"
{$name} �l
���ЃT�[�r�X�������p�����������肪�Ƃ��������܂��B
���o�^����{$day1}���1�������o�߂�������l�ɃA���P�[�g�̂����͂����肢���Ă���܂��B
���񓚂���������ꍇ��{$day2}�܂łɕԐM���Ă���������ƍK���ł��B
�����X�������肢�v���܂��B

{$today} �T�[�r�X���P�S��
"$

$str:deploy