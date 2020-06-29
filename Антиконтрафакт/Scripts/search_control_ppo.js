function init() {  
    var myMap = new ymaps.Map('map', {
        center: [58.010450, 56.229434],
        zoom: 13,
        controls: ['geolocationControl','zoomControl']
    });
    
    // Создадим экземпляр элемента управления «поиск по карте»
    // с установленной опцией провайдера данных для поиска по организациям.
    var searchControl = new ymaps.control.SearchControl({
        options: {
            provider: 'yandex#search'
        }
    });
    
    myMap.controls.add(searchControl);
    
    // Программно выполним поиск определённых кафе в текущей
    // прямоугольной области карты.
    searchControl.search('Магазин');
}

ymaps.ready(init);
