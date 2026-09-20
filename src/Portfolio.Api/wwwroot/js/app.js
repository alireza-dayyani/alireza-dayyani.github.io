const skillIcons = [
    "bi-braces-asterisk",
    "bi-diagram-3",
    "bi-database",
    "bi-broadcast-pin",
    "bi-box-seam",
    "bi-activity"
];

const escapeHtml = (value) => String(value)
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#039;");

async function loadPortfolio() {
    const sources = ["/api/portfolio", "/data/portfolio.json"];

    for (const source of sources) {
        try {
            const response = await fetch(source, { headers: { Accept: "application/json" } });
            if (response.ok && response.headers.get("content-type")?.includes("application/json")) {
                return {
                    content: await response.json(),
                    isApiConnected: source.startsWith("/api/")
                };
            }
        } catch {
            // The JSON fallback keeps the site usable on static hosting.
        }
    }

    throw new Error("Portfolio content is currently unavailable.");
}

function renderProfile(profile) {
    document.querySelector("#availability").textContent = profile.availability;
    document.querySelector("#profile-summary").textContent = profile.summary;
    document.querySelector("#location").textContent = profile.location;

    const githubLink = document.querySelector("#github-link");
    githubLink.href = profile.githubUrl ?? profile.gitHubUrl;

    const emailLink = document.querySelector("#email-link");
    emailLink.href = `mailto:${profile.email}`;
    document.querySelector("#email-text").textContent = profile.email;

    const phoneLink = document.querySelector("#phone-link");
    phoneLink.href = `tel:${profile.phone.replace(/[^+\d]/g, "")}`;
    document.querySelector("#phone-text").textContent = profile.phone;

    if (profile.linkedInUrl) {
        const linkedInLink = document.querySelector("#linkedin-link");
        linkedInLink.href = profile.linkedInUrl;
        linkedInLink.classList.remove("d-none");
    }
}

function renderMetrics(metrics) {
    document.querySelector("#metrics").innerHTML = metrics.map((metric) => `
        <div class="metric reveal">
            <strong>${escapeHtml(metric.value)}</strong>
            <span>${escapeHtml(metric.label)}</span>
        </div>
    `).join("");
}

function renderSkills(skills) {
    document.querySelector("#skills").innerHTML = skills.map((group, index) => `
        <div class="col-md-6 col-xl-4 reveal">
            <article class="skill-card">
                <div class="skill-card-header">
                    <i class="bi ${skillIcons[index % skillIcons.length]}" aria-hidden="true"></i>
                    <h3>${escapeHtml(group.category)}</h3>
                </div>
                <div class="tag-list">
                    ${group.items.map((item) => `<span class="tech-tag">${escapeHtml(item)}</span>`).join("")}
                </div>
            </article>
        </div>
    `).join("");
}

function renderProjects(projects) {
    document.querySelector("#projects").innerHTML = projects.map((project) => `
        <article class="project-card reveal">
            <div class="row g-0">
                <div class="col-lg-6">
                    <div class="project-content">
                        <span class="project-eyebrow">${escapeHtml(project.eyebrow)}</span>
                        <h3>${escapeHtml(project.name)}</h3>
                        <p>${escapeHtml(project.description)}</p>
                        <ul class="project-highlights">
                            ${project.highlights.map((highlight) => `<li>${escapeHtml(highlight)}</li>`).join("")}
                        </ul>
                        <div class="tag-list">
                            ${project.technologies.map((technology) => `<span class="tech-tag">${escapeHtml(technology)}</span>`).join("")}
                        </div>
                    </div>
                </div>
                <div class="col-lg-6">
                    <div class="transport-visual" aria-label="Reliable packet transport illustration">
                        <span class="packet packet-one">REQ:7F2A</span>
                        <span class="packet packet-two">RETRY:01</span>
                        <span class="packet packet-three">ACK:7F2A</span>
                        <span class="packet packet-four">18.4K msg/s</span>
                        <div class="transport-core">
                            <span>
                                <i class="bi bi-router" aria-hidden="true"></i>
                                <strong>TIR / UDP</strong>
                                <small>transport.online</small>
                            </span>
                        </div>
                    </div>
                </div>
            </div>
        </article>
    `).join("");
}

function renderExperience(experience) {
    document.querySelector("#experience-list").innerHTML = experience.map((item) => `
        <article class="timeline-item reveal">
            <div class="timeline-date">${escapeHtml(item.start)}<br>— ${escapeHtml(item.end)}</div>
            <div class="timeline-content">
                <h3>${escapeHtml(item.role)}</h3>
                <span class="timeline-company">${escapeHtml(item.company)} · ${escapeHtml(item.location)}</span>
                <p>${escapeHtml(item.summary)}</p>
                <ul>
                    ${item.achievements.map((achievement) => `<li>${escapeHtml(achievement)}</li>`).join("")}
                </ul>
            </div>
        </article>
    `).join("");
}

function observeReveals() {
    const elements = document.querySelectorAll(".reveal:not(.is-visible)");

    if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) {
        elements.forEach((element) => element.classList.add("is-visible"));
        return;
    }

    const observer = new IntersectionObserver((entries) => {
        entries.forEach((entry) => {
            if (entry.isIntersecting) {
                entry.target.classList.add("is-visible");
                observer.unobserve(entry.target);
            }
        });
    }, { threshold: 0.12 });

    elements.forEach((element) => observer.observe(element));
}

function setupNavigation() {
    const navigation = document.querySelector(".navbar");
    const updateNavigation = () => navigation.classList.toggle("scrolled", window.scrollY > 24);
    updateNavigation();
    window.addEventListener("scroll", updateNavigation, { passive: true });

    document.querySelectorAll("#mainNav .nav-link, #mainNav .btn").forEach((link) => {
        link.addEventListener("click", () => {
            const openMenu = document.querySelector("#mainNav.show");
            if (openMenu && window.bootstrap) {
                window.bootstrap.Collapse.getOrCreateInstance(openMenu).hide();
            }
        });
    });
}

function setupContactForm() {
    const form = document.querySelector("#contact-form");
    const status = document.querySelector("#form-status");
    const submitButton = form.querySelector("button[type='submit']");

    form.addEventListener("submit", async (event) => {
        event.preventDefault();
        status.className = "form-status";

        if (!form.checkValidity()) {
            form.classList.add("was-validated");
            status.textContent = "Please complete each field correctly.";
            status.classList.add("error");
            return;
        }

        submitButton.disabled = true;
        status.textContent = "Sending…";

        const payload = Object.fromEntries(new FormData(form).entries());

        try {
            const response = await fetch("/api/contact", {
                method: "POST",
                headers: { "Content-Type": "application/json", Accept: "application/json" },
                body: JSON.stringify(payload)
            });

            if (!response.ok) {
                const problem = await response.json().catch(() => null);
                const firstError = problem?.errors ? Object.values(problem.errors).flat()[0] : null;
                throw new Error(firstError ?? "The request could not be sent.");
            }

            form.reset();
            form.classList.remove("was-validated");
            status.textContent = "Thanks — your request has been queued.";
            status.classList.add("success");
        } catch (error) {
            status.textContent = `${error.message} You can still reach me directly by email.`;
            status.classList.add("error");
        } finally {
            submitButton.disabled = false;
        }
    });
}

async function initialize() {
    document.querySelector("#current-year").textContent = new Date().getFullYear();
    setupNavigation();
    setupContactForm();

    try {
        const { content, isApiConnected } = await loadPortfolio();
        renderProfile(content.profile);
        renderMetrics(content.metrics);
        renderSkills(content.skills);
        renderProjects(content.projects);
        renderExperience(content.experience);

        if (!isApiConnected) {
            document.querySelector("#api-meta").classList.add("d-none");
        }
    } catch (error) {
        console.error(error);
    }

    observeReveals();
}

initialize();
