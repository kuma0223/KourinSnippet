InputDialog["名前", "登録日", "期限"]

$name = $$Input1
$day1 = $$Input2
$day2 = $$Input3
$today = format["{0:yyyy年MM月dd日}", date]

$str = $"
{$name} 様
弊社サービスをご利用いただきありがとうございます。
ご登録日の{$day1}より1ヵ月が経過した会員様にアンケートのご協力をお願いしております。
ご回答いただける場合は{$day2}までに返信していただけると幸いです。
何卒宜しくお願い致します。

{$today} サービス改善担当
"$

$str:deploy