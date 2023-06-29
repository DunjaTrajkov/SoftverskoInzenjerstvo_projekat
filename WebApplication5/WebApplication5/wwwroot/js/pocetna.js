var myF = function () {
        var dugmici = document.querySelectorAll(".promeni-btn");
        for (let i = 0; i < 5; i++)
        {
            dugmici[i].onclick = (ev => {
                prikaziTxtbox(ev);
            })
        }

        function getFile(filename)
        {
            fetch("fajlovi/" + filename + ".txt")
                .then(function (response) {
                    return response.text();
                })
                .then(function (data) {
                    localStorage.setItem(filename, data);
                })
                .catch(function (error) {
                    console.log(error);
                })
        }

        function onLoad() {
            var p = document.getElementById("O nama");
            p.innerHTML = localStorage.getItem("O nama");
            var p = document.getElementById("Informacije o dostavi");
            p.innerHTML = localStorage.getItem("Informacije o dostavi");
            var p = document.getElementById("Politika privatnosti");
            p.innerHTML = localStorage.getItem("Politika privatnosti");
            var p = document.getElementById("Korisnicka usluga");
            p.innerHTML = localStorage.getItem("Korisnicka usluga");
            var p = document.getElementById("Uslovi koriscenja");
            p.innerHTML = localStorage.getItem("Uslovi koriscenja");
        }
        if (localStorage.length === 0) {
            getFile("O nama");
            getFile("Politika privatnosti");
            getFile("Informacije o dostavi");
            getFile("Korisnicka usluga");
            getFile("Uslovi koriscenja");
        }
        onLoad();
    }
    myF();

    function prikaziTxtbox(ev) {
        const rod = ev.currentTarget.parentElement;
        const dugme = rod.querySelector(".promeni-btn");
        var txtA = rod.querySelector("textarea");
        if (txtA != null)
            return;
        var txt = dugme.value;
        var paragraf = document.getElementById(txt);
        paragraf.hidden = true;
        var textArea = document.createElement("textarea");
        textArea.name = "promeni";
        textArea.rows = 10;
        textArea.cols = 100;
        textArea.value = paragraf.innerHTML;
        rod.appendChild(textArea);
        var submitbtn = document.createElement("button");
        submitbtn.innerHTML = "Potvrdi promenu";
        submitbtn.value = "Potvrdi";
        submitbtn.className = "promeni-btn";
        submitbtn.onclick = (ev) => {
            var parent = ev.currentTarget.parentElement;
            var txta = parent.querySelector("textarea[name=promeni]");
            var submit = parent.querySelector("button[value=Potvrdi]");
            var cancel = parent.querySelector("button[value=Otkazi]");
            var dugme = parent.querySelector(".promeni-btn");
            localStorage.setItem(dugme.value, txta.value);
            paragraf.innerHTML = txta.value;
            paragraf.hidden = false;
            parent.removeChild(txta);
            parent.removeChild(submit);
            parent.removeChild(cancel);
        }
        rod.appendChild(submitbtn);
        var cancelbtn = document.createElement("button");
        cancelbtn.innerHTML = "Otkazi promenu";
        cancelbtn.value = "Otkazi";
        cancelbtn.className = "promeni-btn";
        cancelbtn.onclick = (ev) => {
            var txta = document.querySelector("textarea[name=promeni]");
            var parent = txta.parentElement;
            var submit = parent.querySelector("button[value=Potvrdi]");
            var cancel = parent.querySelector("button[value=Otkazi]");
            paragraf.hidden = false;
            parent.removeChild(txta);
            parent.removeChild(submit);
            parent.removeChild(cancel);
        }
        rod.appendChild(cancelbtn);
    }