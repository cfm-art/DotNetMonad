Monad for .Net
==

.Netでモナドっぽいことをするためのクラス群。

## ライセンス
MIT

## 各種モナドもどきたち

### Optinal
HaskellでいうMaybeモナド。

```cs
var maybeText = Optional.Just("text");  // Just "text"

var maybeText2 = maybeText.Map(text => text + "append"); // Just "textappend"

maybeText2.IfPresent(
    text => Console.WriteLine(text),    // textappend
    () => Console.WriteLinee("not reach.")
);
```

### Either
Eitherモナド。

```cs
var left = Either<Exception, string>.Left(new Excception("error")); // Left Exception "error"
var right = Either<Exception, string>.Right("right");  // Right "right"

var right2 = right.Map(r => r + "!!");   // Right "right!!"

right2.IfRight(
    r => Console.WriteLine(r),  // right!!
    l => Console.WriteLine("not reach.")
);

left.IfLeft(
    l => Console.WriteLine(l.ToString()),   // error
    r => Console.WriteLine("not reach")
);
```

### Bool
Bool専用のOptional。


