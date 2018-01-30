namespace markb {
    export function ready(callback: EventListener): void {
        if (document.readyState === "interactive" || document.readyState === "complete") {
            callback(new Event("DOMContentLoaded"));
        } else {
            document.addEventListener("DOMContentLoaded", callback);
        }
    }

    export function init(): void {
        let e: HTMLElement = document.getElementById("e");

        if (e) {
            let mt: string = String.fromCharCode(109, 97, 105, 108, 116, 111, 58);
            let v: string[] = "109#101#64#109#97#114#107#98#46#99#111#46#117#107".split("#");
            let t: string = v.map(cc => String.fromCharCode(parseInt(cc, 10))).join("");
            e.innerHTML = `<a class=\"email-link\" href=\"${mt}${t}\">${t}</a>`;
        }
    }
}

markb.ready(markb.init);