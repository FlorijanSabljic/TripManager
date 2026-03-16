document.addEventListener('DOMContentLoaded', () => {

    let selectCountryElement = document.querySelector('#country');
    let selectCityElement = document.querySelector('#city');



    selectCountryElement.addEventListener('change', () => {

        let country = selectCountryElement.value;
        const url = `/Trip/GetCities?country=${encodeURIComponent(country)}`;

        if (country === '') {
            selectCityElement.innerHTML = '<option value="">Select City</option>';
            selectCityElement.disabled = true;
            return;
        };

        fetch(url)
            .then(response => response.json())
            .then(data => {
                /*console.log('Fetched cities:', data);*/
                selectCityElement.innerHTML = '<option value="">Select City</option>';
                data.forEach(city => {
                    let option = document.createElement('option');
                    option.value = city;
                    option.textContent = city;
                    selectCityElement.appendChild(option);
                });
                selectCityElement.disabled = false;
            })
            .catch(error => {
                console.error('Error fetching cities:', error);
            });

    });
});
