# project-lighthouse-social

C# ile uçtan uca bir Web projesi. Konu, dünya üzerindeki deniz fenerlerine ait fotoğrafların paylaşıldığı, yorumlandığı ve puanlandığı bir sosyal platform. _(Proje ilerleyişi için Youtube kanalında da bir seri hazırlamak istiyorum)_

## Proje Konusu

Deniz Feneri meraklıları için bir sosyal paylaşım platformu geliştirmek.

## Projenin Genel Özellikleri

- Platform kullanıcıları çektikleri deniz feneri **fotoğraflarını** paylaşabilirler.
- Kullanıcılar dünyanın dört bir yanında yer alan deniz fenerleri hakkında **kapsamlı ve detaylı bilgileri**   öğrenebilirler.
- Kullanıcılar deniz feneri fotoğraflarına **yorum** bırakabilir ve puanlayabilirler.

## Amaçlar

Bu projeyi geliştirmenin temel amaçları aşağıda listelenmiştir.

- C# ve .Net platformunu proje geliştirerek tanımak.
- Düzenli olarak refactoring uygulayıp kodu iyileştirmek.
- AI Asistanlarından yararlanmak _(minimum ölçüde)_
- Temel yazılım prensiplerini keşfetmek, uygulamak ve sorgulamak.

## Zorluklar _(Challenges)_

- Kullanıcıların paylaştığı fotoğrafları nasıl ve nerede tutacağız? _(Boyut, depolama yeri, yazma/okuma hızları, dağıtık topoloji kullanımları)_
- Kullanıcı yorumlarının denetlenmesi ve istenmeyen ifadelerin engellenmesi otonom şekilde nasıl sağlanabilir?
- Bir fotoğrafın doğru deniz fenerine ait olduğunu nasıl tespit edebiliriz?
- Fotoğraflardaki özgünlüğü anlamak için kategorilendirme veya tag'leme aşamasında AI araçlarından nasıl yararlanabiliriz?
- Çok sayıda kullanıcının farkl lokasyonlardan fotoğraf yüklemesi halinde fotoğrafın analizi, doğrulanması, sınıflandırılması gibi hizmetlerin sistemin genelini etkilemeden en hızlı şekilde yapılması nasıl sağlanır?

## İçerik Planı

- [ ] **Bölüm 00 Proje Tanıtımı ve Solution Açılması:** Projenin amacı anlatılır, teknoloji seçimine değinilir ve Solution açılır.
- [ ] **Bölüm 01 Domain Model Tasarımı:** User, Lighthouse, Photo, Comment vb temel sınıflar belirlenir.
- [ ] **Bölüm 02 CRUD Sözleşmelerinin İnşaası:** Repository/Service katmanları için interface tasarımları ve abstraction katmanı.
- [ ] ...

## Kontrol Listesi

- [ ] C# dilinin temel özelliklerine yer verildi mi?
- [ ] OOP prensipleri uygulandı mı?
- [ ] SOLID prensiplerine yer verildi mi?
- [ ] Projede en az bir Rest tabanlı Web Api kullanıldı mı?
- [ ] Projede gRPC tabanlı bir servis kullanıldı mı?
- [ ] Razor tabanlı Web uygulaması geliştirildi mi?
- [ ] Farklı dillerde yazılmış servisler kullanıldı mı?
- [ ] Bir dağıtık sistem kurgusu tesis edildi mi?
- [ ] Dağıtık sistem kurgusu tesis edildiyse relaibility için gerekli tedbirler alındı mı?
- [ ] Dağıtık sistem kurugusu için izleme, loglama, alarm mekanizmaları vs kullanıldı mı?
