function main() {
    const arr = [];

    const ultima_conexion = $(".player-description p")[1].innerText;

    arr.push({ ultima_conexion })

    $$(".game").forEach(a => {
        const game = a.querySelector(".game-header-title").innerText
        const title = a.querySelectorAll(".game-content .game-stat-title")
        const count = a.querySelectorAll(".game-content .game-stat-count")
        const stats = []
        for (let i = 0; i < title.length; i++) {
            stats.push({ estadistica: title[i].innerText, cantidad: count[i].innerText })
        }
        arr.push({ game, stats })
    })
    return arr;
}


console.log(main())