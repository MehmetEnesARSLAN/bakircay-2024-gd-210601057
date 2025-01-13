Oyunun linki: https://play.unity.com/en/games/cc86da5e-32ed-4a65-95a9-1831f9f71b9d/match-3d

Bu projede, Unity oyun motorunu kullanarak bir eşleştirme oyunu geliştiriyoruz. Oyunun temel amacı, kullanıcıların oyun alanındaki nesneleri doğru şekilde eşleştirerek puan toplaması üzerine kurulu. Oyun, WebGL platformunda çalışacak şekilde yapılandırılıyor ve bu sayede tarayıcı tabanlı bir deneyim sunuyor. İşte proje geliştirme sürecindeki adımlar ve karşılaşılan problemlerin çözümleri:

Oyun Mekaniği: Eşleştirme Sistemi

Oyunda, farklı türde nesneler bulunuyor ve kullanıcı bu nesneleri eşleştirme alanına doğru sürükleyip bırakabiliyor. Eğer eşleşen iki nesne doğru bir çiftse, oyuncuya puan kazandırılıyor ve bu nesneler sahneden kaldırılıyor. Yanlış eşleşmelerde ise nesneler oyun alanının merkezine doğru itiliyor.

Eşleştirme Alanı:

İki sabit Transform noktası (snap point) tanımlanıyor. Kullanıcı, nesneleri bu noktalara taşıyarak eşleşme gerçekleştirebiliyor.
Eğer iki nokta dolduğunda nesneler aynı ise başarılı bir eşleşme gerçekleşiyor, değilse nesneler oyun alanının merkezine doğru itiliyor.
Nesne Yok Etme ve Animasyon:
Doğru eşleşme durumunda, nesneler küçülerek eşleşme alanının merkezine doğru hareket ediyor ve ardından kayboluyor.
Yanlış eşleşme durumunda nesneler belirli bir gecikmeyle merkezden uzaklaştırılıyor.
Teleport (Işınlama) Özelliği

Oyuna önemli bir mekanik olan ışınlama özelliği ekleniyor. Bu özellik sayesinde:

Kullanıcı, bir butona tıkladığında belirli bir süre için "ışınlama modu" aktif hale geliyor.
Işınlama modu etkinleştirildiğinde, kullanıcı oyun alanındaki bir nesneye tıklayarak bu nesneyi eşleşme alanında önceden belirlenen hedef bir noktaya ışınlayabiliyor.
Çözülen Problemler:

Işınlanan nesneler, fareyle sürüklenmeye devam ediyordu. Bu sorunu çözmek için ışınlanan nesnelerin pozisyonları kilitleniyor ve fiziksel etkileşimleri durduruluyor.
Işınlanan nesnelerin doğru prefab’dan yeniden oluşturulmasını sağlıyor ve eşleşme mekanizmasıyla uyumlu hale getiriyoruz.
İki Katı Puan Özelliği
Oyuna eklenen bir diğer özellik, 2x Puan Butonu oluyor.

Oyun sırasında bu butona basıldığında belirli bir süre boyunca aktif hale geliyor ve bu süre boyunca yapılan her eşleşmede oyuncu iki katı puan kazanıyor.
UI Yönetimi: Kullanıcı Arayüzü

Oyunda, kullanıcı deneyimini geliştirmek için bir UI sistemi tasarlanıyor.

Puan Sistemi:

Her başarılı eşleşmede oyuncuya puan kazandırılıyor.
Kazanılan puan, UI üzerinde gerçek zamanlı olarak güncelleniyor.
Işınlama Butonu:

Işınlama modu için bir buton ekleniyor. Bu buton, belirli bir süre boyunca aktif kalıyor ve ardından devre dışı kalıyor.
Cooldown süresi tamamlandıktan sonra yeniden etkinleşiyor.
Sıfırlama Butonu:

Oyunu yeniden başlatma seçeneği sunuluyor. Bu buton, oyun alanındaki tüm nesneleri temizleyerek yeni nesnelerin spawn edilmesini sağlıyor.
Spawn (Nesne Üretimi) Sistemi

Her yeni turda oyun alanına farklı çiftler spawn ediliyor.

Farklı Prefab’lar:

Her eşleşme için benzersiz prefab’lar tanımlanıyor.
Aynı prefab’ların yanlış bir şekilde oyun alanı dışına itilmesi problemi çözülüyor.
Rastgele Konum:

Nesnelerin oyun alanında rastgele ancak düzgün bir şekilde dağılmasını sağlıyoruz.
Karşılaşılan Problemler ve Çözümler

Proje geliştirme sırasında aşağıdaki problemlerle karşılaşıyoruz ve bunlara çözümler üretiyoruz:

Büyük Dosyalar:
WebGL build dosyaları GitHub'ın dosya boyutu sınırını aşıyor. Bu sorunu çözmek için Play.Unity.com kullanılarak dosyalar depolanıyor.
Fiziksel Davranış Sorunları:
Işınlanan nesneler, fiziksel olarak sürüklenmeye devam ediyordu. Bu sorunu nesnelerin pozisyonlarını kilitleyerek ve fiziksel etkileşimlerini devre dışı bırakarak çözüyoruz.
Prefab Yönetimi:
Işınlanan nesnelerin doğru prefab ile yeniden oluşturulmasını sağlıyor ve bu nesneleri eşleşme mekanizmasıyla uyumlu hale getiriyoruz.


