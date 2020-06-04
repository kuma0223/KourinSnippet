InputDialog["名前", "登録日", "期限"]

$name = $$Input1
$day1 = $$Input2
$day2 = $$Input3

$str = ""
$str = $str & $name & "様、いつもご利用いただきありがとうございます。\n"
$str = $str & "ご登録日の" & $day1 & "より1カ月が経過いたしましたので、アンケートのご協力をお願いします。\n"
$str = $str & "ご回答いただける場合は" & $day2 & "までに返信していただけると幸いです。\n"
$str = $str & "よろしくお願いいたします。\n\n"
$str = $str & format["{0:yyyy年MM月dd日}", date] & " サービス改善担当"
$str