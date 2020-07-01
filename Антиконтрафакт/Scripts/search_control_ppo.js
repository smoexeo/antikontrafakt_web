function init() {  
    var geolocation = ymaps.geolocation,
        myMap = new ymaps.Map('map', {
        center: [58.010450, 56.229434],
        zoom: 13,
        controls: ['geolocationControl','zoomControl']
    });
    geolocation.get({
        provider: 'browser',
	mapStateAutoApply: true
    }).then(function (result) {
        // Если браузер не поддерживает эту функциональность, метка не будет добавлена на карту.
        result.geoObjects.options.set('preset', 'islands#redCircleIcon');
        myMap.geoObjects.add(result.geoObjects);
	myMap.setZoom(13);
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
