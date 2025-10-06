window.metaManager = {
    setMeta: (name, content) => {
        if (!content) return;
        let element = document.querySelector(`meta[name='${name}']`) || document.querySelector(`meta[property='${name}']`);
        if (element) {
            element.setAttribute("content", content);
        } else {
            element = document.createElement("meta");
            if (name.startsWith("og:") || name.startsWith("twitter:")) {
                element.setAttribute("property", name); // Open Graph uses property
            } else {
                element.setAttribute("name", name);
            }
            element.setAttribute("content", content);
            document.head.appendChild(element);
        }
    },
    setTitle: (title) => {
        if (title) document.title = title;
    }
};