/* ═══════════════════════════════════════════
   AUREVIA — app.js
   Search · Detail Gallery · Market Widgets
═══════════════════════════════════════════ */
'use strict';

const API_KEY  = '47751142damsh49a735a0ef5a185p1f08afjsn13716bb0e823';
const API_HOST = 'booking-com15.p.rapidapi.com';
const BASE     = `https://${API_HOST}/api/v1`;
const HDRS     = { 'x-rapidapi-key': API_KEY, 'x-rapidapi-host': API_HOST };

const S = {
  adults:2, children:0, rooms:1,
  page:1, totalPages:1,
  hotels:[], destId:'', sType:'CITY',
  ci:'', co:'', city:''
};

const $   = id => document.getElementById(id);
const ts  = () => new Date().toLocaleTimeString('en-GB',{hour:'2-digit',minute:'2-digit'});
const fd  = d  => d.toISOString().split('T')[0];
const fmtN = (n,d=0) => Number(n).toLocaleString('en-US',{minimumFractionDigits:d,maximumFractionDigits:d});
const nightCount = (a,b) => !a||!b?0:Math.max(0,Math.round((new Date(b)-new Date(a))/86400000));

/* ── Navbar scroll ── */
function initNavScroll() {
  const nb = $('nav') || document.querySelector('.nav'); if(!nb) return;
  const onScroll = () => nb.classList.toggle('scrolled', window.scrollY > 60);
  window.addEventListener('scroll', onScroll, { passive: true });
  onScroll();
}

/* ── Dates ── */
function initDates() {
  const ci = $('checkin'), co = $('checkout'); if(!ci) return;
  const t = new Date();
  const d1 = new Date(); d1.setDate(t.getDate()+1);
  const d2 = new Date(); d2.setDate(t.getDate()+4);
  ci.min = fd(t); ci.value = fd(d1);
  co.min = fd(d1); co.value = fd(d2);
  ci.addEventListener('change', () => {
    const nd = new Date(ci.value); nd.setDate(nd.getDate()+1);
    co.min = fd(nd);
    if(co.value <= ci.value) co.value = fd(nd);
  });
}

/* ── Steppers ── */
const SC = {adults:{min:1,max:8},children:{min:0,max:6},rooms:{min:1,max:5}};
function step(k, dir) {
  const c = SC[k];
  S[k] = Math.min(c.max, Math.max(c.min, S[k]+dir));
  const el = $(k+'Val'); if(!el) return;
  el.textContent = S[k];
  el.style.color = 'var(--gold)'; el.style.transform = 'scale(1.3)';
  setTimeout(()=>{ el.style.color=''; el.style.transform=''; }, 230);
}

/* ── City Autocomplete ── */
let cityTimer = null;
function initCity() {
  const inp = $('cityInput'), sugs = $('suggestions'); if(!inp) return;
  inp.addEventListener('input', () => {
    clearTimeout(cityTimer);
    const q = inp.value.trim();
    if(q.length < 2) { sugs.classList.remove('open'); return; }
    sugs.innerHTML = '<div class="sug-loader">Searching…</div>';
    sugs.classList.add('open');
    cityTimer = setTimeout(() => fetchSugs(q), 420);
  });
  document.addEventListener('click', e => {
    if(!e.target.closest('#destWrap')) sugs.classList.remove('open');
  });
}

async function fetchSugs(q) {
  const sugs = $('suggestions');
  try {
    const r = await fetch(`${BASE}/hotels/searchDestination?query=${encodeURIComponent(q)}`, {headers:HDRS});
    const d = await r.json();
    if(!d.data?.length) { sugs.innerHTML='<div class="sug-loader">No destinations found.</div>'; return; }
    sugs.innerHTML = '';
    d.data.slice(0,8).forEach(item => {
      const div = document.createElement('div');
      div.className = 'sug-row';
      div.innerHTML = `<div class="sug-name">${item.label||item.city_name||item.name||'—'}</div>
                       <div class="sug-sub">${item.country||''} · ${item.dest_type||'city'}</div>`;
      div.addEventListener('click', () => {
        $('cityInput').value = item.label||item.city_name||item.name;
        $('destId').value    = item.dest_id||'';
        $('searchType').value= (item.dest_type||'CITY').toUpperCase();
        S.destId  = item.dest_id||'';
        S.sType   = (item.dest_type||'CITY').toUpperCase();
        S.city    = item.label||item.city_name||item.name;
        sugs.classList.remove('open');
      });
      sugs.appendChild(div);
    });
  } catch { sugs.innerHTML = '<div class="sug-loader">Error loading suggestions.</div>'; }
}

/* ══ HOTEL SEARCH ══ */
async function searchHotels() {
  const did = $('destId')?.value || S.destId;
  const ci  = $('checkin')?.value;
  const co  = $('checkout')?.value;
  if(!did) { shake($('cityInput')); return; }
  if(!ci)  { shake($('checkin'));   return; }
  if(!co)  { shake($('checkout')); return; }

  S.destId = did; S.sType = $('searchType')?.value || S.sType;
  S.ci = ci; S.co = co; S.city = $('cityInput')?.value || S.city; S.page = 1;

  showLoader();
  $('results')?.scrollIntoView({behavior:'smooth', block:'start'});
  const btn = $('searchBtn');
  if(btn) { btn.disabled=true; btn.textContent='Searching…'; }

  try {
    const ages = S.children>0 ? Array(S.children).fill('5').join(',') : '0';
    const p = new URLSearchParams({
      dest_id:S.destId, search_type:S.sType, adults:S.adults,
      children_age:ages, room_qty:S.rooms, page_number:S.page,
      units:'metric', temperature_unit:'c', languagecode:'en-us',
      currency_code:'USD', arrival_date:ci, departure_date:co
    });
    const r = await fetch(`${BASE}/hotels/searchHotels?${p}`, {headers:HDRS});
    const d = await r.json();
    hideLoader();
    if(!d.data?.hotels?.length) { showEmpty(); return; }
    S.hotels = d.data.hotels;
    S.totalPages = Math.max(1, Math.ceil((d.data.meta?.total_count||S.hotels.length)/20));
    renderHotels();
  } catch(e) { hideLoader(); showErr(e.message); }
  finally {
    if(btn) {
      btn.disabled = false;
      btn.innerHTML = `<svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2"><circle cx="11" cy="11" r="8"/><path d="m21 21-4.35-4.35"/></svg> Search Hotels`;
    }
  }
}

function renderHotels() {
  const grid = $('hotelGrid'), hdr = $('resultsHead'); if(!grid) return;
  grid.innerHTML = '';
  if(hdr) hdr.style.display = 'block';
  hide('errPanel'); hide('emptyPanel');
  setText('cityLabel', S.city);
  setText('resultsSub', `${S.ci} – ${S.co} · ${S.hotels.length} properties · USD / night`);
  sortedH().forEach((h,i) => grid.appendChild(buildCard(h,i)));
  const pag = $('pag');
  if(pag) {
    pag.style.display = S.totalPages>1 ? 'flex' : 'none';
    setText('pageLabel', `Page ${S.page} of ${S.totalPages}`);
    if($('prevBtn')) $('prevBtn').disabled = S.page<=1;
    if($('nextBtn')) $('nextBtn').disabled = S.page>=S.totalPages;
  }
}

function sortedH() {
  const v = $('sortSel')?.value || 'price_asc';
  return [...S.hotels].sort((a,b)=>{
    const pa = a.property?.priceBreakdown?.grossPrice?.value ?? 9e9;
    const pb = b.property?.priceBreakdown?.grossPrice?.value ?? 9e9;
    const ra = a.property?.reviewScore ?? 0, rb = b.property?.reviewScore ?? 0;
    if(v==='price_asc')  return pa-pb;
    if(v==='price_desc') return pb-pa;
    if(v==='rating_desc')return rb-ra;
    return 0;
  });
}
function sortHotels() { renderHotels(); }

function buildCard(h, idx) {
  const p     = h.property || {};
  const name  = p.name || 'Hotel';
  const stars = Math.min(p.propertyClass||0, 5);
  const photo = p.photoUrls?.[0] || '';
  const price = p.priceBreakdown?.grossPrice?.value;
  const id    = h.hotel_id || p.id || '';

  const starsHtml = Array.from({length:5}, (_,i) =>
    `<span class="${i<stars?'s-full':'s-empty'}">★</span>`).join('');

  const card = document.createElement('article');
  card.className = 'hotel-card';
  card.style.animationDelay = `${idx * 0.055}s`;
  card.innerHTML = `
    <div class="hcard-img">
      ${photo
        ? `<img src="${photo}" alt="${name}" loading="lazy" onerror="this.parentNode.innerHTML='<div class=hcard-placeholder>🏨</div>'">`
        : `<div class="hcard-placeholder">🏨</div>`}
    </div>
    <div class="hcard-body">
      <div class="hcard-stars">${starsHtml}</div>
      <h3 class="hcard-name">${name}</h3>
      <div class="hcard-price-row">
        <span class="hcard-price">${price ? `$${fmtN(price,0)}` : '—'}</span>
        <span class="hcard-per">/ night</span>
      </div>
    </div>`;
  card.addEventListener('click', () => goDetail(id));
  return card;
}

function goDetail(id) {
  const p = new URLSearchParams({hotel_id:id, checkin:S.ci, checkout:S.co, adults:S.adults, children:S.children, rooms:S.rooms});
    window.location.href = `Detail?${p}`;
}

async function changePage(dir) {
  S.page = Math.max(1, Math.min(S.totalPages, S.page+dir));
  showLoader();
  window.scrollTo({top: $('results').offsetTop-80, behavior:'smooth'});
  try {
    const ages = S.children>0 ? Array(S.children).fill('5').join(',') : '0';
    const p = new URLSearchParams({
      dest_id:S.destId, search_type:S.sType, adults:S.adults, children_age:ages,
      room_qty:S.rooms, page_number:S.page, units:'metric', temperature_unit:'c',
      languagecode:'en-us', currency_code:'USD', arrival_date:S.ci, departure_date:S.co
    });
    const r = await fetch(`${BASE}/hotels/searchHotels?${p}`, {headers:HDRS});
    const d = await r.json();
    hideLoader();
    if(d.data?.hotels) { S.hotels=d.data.hotels; renderHotels(); }
  } catch(e) { hideLoader(); showErr(e.message); }
}

/* ── helpers ── */
function showLoader() { show('loader'); if($('hotelGrid'))$('hotelGrid').innerHTML=''; hide('errPanel'); hide('emptyPanel'); hide('resultsHead'); if($('pag'))$('pag').style.display='none'; }
function hideLoader() { hide('loader'); }
function showEmpty()  { show('emptyPanel'); }
function showErr(m)   { show('errPanel'); setText('errMsg', m||'Something went wrong.'); }
function show(id)     { const e=$(id); if(e)e.style.display='block'; }
function hide(id)     { const e=$(id); if(e)e.style.display='none'; }
function setText(id,v){ const e=$(id); if(e)e.textContent=v; }
function shake(el)    { if(!el)return; el.style.outline='1px solid var(--gold)'; setTimeout(()=>el.style.outline='',1500); }

/* ══════════════════════════════════════════
   DETAIL PAGE
══════════════════════════════════════════ */
async function initDetail() {
  const dc=$('dContent'), dl=$('dLoading'), de=$('dError'); if(!dc) return;
  const qp = new URLSearchParams(window.location.search);
  const hid      = qp.get('hotel_id');
  const ci       = qp.get('checkin')  || '';
  const co       = qp.get('checkout') || '';
  const adults   = qp.get('adults')   || '2';
  const children = qp.get('children') || '0';
  const rooms    = qp.get('rooms')    || '1';

  if(!hid) { dl.style.display='none'; de.style.display='flex'; return; }

  setText('bsCI', ci); setText('bsCO', co);
  setText('bsGuests', `${adults} adult${+adults>1?'s':''} · ${rooms} room${+rooms>1?'s':''}`);

  try {
    const ages = +children>0 ? Array(+children).fill('5').join(',') : '0';
    const p = new URLSearchParams({
      hotel_id:hid, arrival_date:ci, departure_date:co, adults,
      children_age:ages, room_qty:rooms, units:'metric',
      temperature_unit:'c', languagecode:'en-us', currency_code:'USD'
    });
    const r = await fetch(`${BASE}/hotels/getHotelDetails?${p}`, {headers:HDRS});
    const data = await r.json();
    if(dl) dl.style.display='none';
    if(!data.data) { de.style.display='flex'; return; }
    dc.style.display = 'block';
    buildDetail(data.data, {ci, co, adults, children, rooms});
  } catch {
    if(dl) dl.style.display='none';
    if(de) de.style.display='flex';
  }
}

function buildDetail(d, b) {
  const name    = d.hotel_name || d.name || '—';
  const addr    = [d.address, d.city, d.country].filter(Boolean).join(', ');
  const stars   = Math.min(d.class||d.stars||0, 5);
  const score   = d.review_score || d.reviewScore;
  const word    = d.review_score_word || d.reviewScoreWord || '';
  const revN    = d.review_nr || d.reviewCount || 0;
  const desc    = d.description_translations?.[0]?.description || d.description || d.hotel_description || 'No description available for this property.';
  const price   = d.composite_price_breakdown?.gross_amount?.value || d.price_breakdown?.gross_price || null;
  const lat     = d.latitude||''; const lon = d.longitude||'';
  const nights  = nightCount(b.ci, b.co);
  const facils  = d.facilities_block?.facilities || d.facilities || [];

  document.title = `${name} — Aurevia`;

  /* Gallery photos */
  const photos = [
    ...(d.main_photo_url ? [d.main_photo_url] : []),
    ...(d.photos?.map(p=>p.url_original||p.url) || []),
    ...(Object.values(d.rooms||{}).flatMap(r=>(r.photos||[]).map(p=>p.url_original||p.url)))
  ].filter(Boolean).slice(0, 6);

  buildAsymmetricGallery(photos, name);

  /* Name & rating */
  setText('dName', name);
  const starsEl = $('dStars');
  if(starsEl) starsEl.textContent = '★'.repeat(stars) + '☆'.repeat(5-stars);

  const scoreEl = $('dScore');
  if(scoreEl && score) {
    scoreEl.textContent = `${Number(score).toFixed(1)} · ${word}`;
    scoreEl.style.display = '';
  }
  setText('dReviews', revN ? `${fmtN(revN)} reviews` : '');

  /* Location */
  setText('dLocation', addr);
  const mapLink = $('dMapLink');
  if(mapLink) mapLink.href = `https://www.google.com/maps/search/${encodeURIComponent(name+' '+addr)}`;

  /* Stay meta */
  const metaEl = $('dStayMeta');
  if(metaEl) {
    const tags = [];
    if(b.ci && b.co) tags.push(`${b.ci} → ${b.co}`);
    if(nights>0)      tags.push(`${nights} nights`);
    if(d.checkin?.from) tags.push(`Check-in from ${d.checkin.from}`);
    metaEl.innerHTML = tags.map(t=>`<span>${t}</span>`).join('');
  }

  /* Feature chips */
  const featEl = $('dFeatures');
  if(featEl) {
    const featureNames = facils.slice(0,10).map(f=>f.name||f);
    featEl.innerHTML = featureNames.map(n=>`<div class="feature-chip"><span>✓</span>${n}</div>`).join('');
  }

  /* Description */
  setText('dDesc', desc);

  /* Facilities list */
  const facEl = $('dFacilities');
  if(facEl) facEl.innerHTML = facils.slice(0,14).map(f=>`<li>${f.name||f}</li>`).join('');

  /* Policies */
  const polEl = $('dPolicies');
  if(polEl) {
    const pols = [
      {l:'Check-in',  v: d.checkin?.from  || '—'},
      {l:'Check-out', v: d.checkout?.until || '—'},
      {l:'Cancellation', v: d.is_free_cancellable ? '✓ Free cancellation' : 'Non-refundable'},
      {l:'Smoking',   v: d.is_smoking_allowed ? 'Permitted' : 'Not permitted'},
      {l:'Pets',      v: d.pet_policy || 'Contact hotel'},
      {l:'Payment',   v: d.payment_details?.partner?.can_pay_at_property ? 'Pay at hotel' : 'Online payment'},
    ];
    polEl.innerHTML = pols.map(p=>`
      <div style="background:rgba(255,255,255,0.03);border:1px solid rgba(255,255,255,0.07);border-radius:8px;padding:10px 14px">
        <div style="font-size:0.62rem;letter-spacing:0.14em;text-transform:uppercase;color:var(--text3);margin-bottom:4px">${p.l}</div>
        <div style="font-size:0.83rem;font-weight:500;color:var(--text)">${p.v}</div>
      </div>`).join('');
  }

  /* Booking sidebar */
  setText('bsPrice', price ? fmtN(price,1) : '—');
  const bsMeta = $('bsMeta');
  if(bsMeta) bsMeta.textContent = score ? `${nights>0?nights+' nights · ':''}${fmtN(revN)} reviews` : (nights>0?`${nights} nights`:'');

  const bsTotal = $('bsTotal');
  if(bsTotal && price && nights>0) {
    const total = price * nights;
    bsTotal.style.display = 'block';
    bsTotal.innerHTML = `
      <div class="bs-total-row"><span>${nights} nights × $${fmtN(price,1)}</span><span>$${fmtN(total,2)}</span></div>
      <div class="bs-total-row final"><span>Total estimate</span><span>$${fmtN(total,1)} USD</span></div>`;
  }

  const bookBtn = $('bsBookBtn');
  if(bookBtn) {
    const url = `https://www.booking.com/hotel/${d.url||''}?checkin=${b.ci}&checkout=${b.co}&group_adults=${b.adults}&no_rooms=${b.rooms}`;
    bookBtn.href = d.url ? url : `https://www.booking.com/searchresults.html?ss=${encodeURIComponent(name)}`;
  }
}

/* ══ ASYMMETRIC GALLERY ══
   Layout:
   | BIG (left, tall) | small1 | small2 |
   |                  |   wide (bottom)  |
══════════════════════════════════════════ */
function buildAsymmetricGallery(photos, name) {
  const container = $('gallery'); if(!container) return;
  container.innerHTML = '';

  if(!photos.length) {
    container.innerHTML = `<div style="grid-column:1/-1;grid-row:1/-1;display:flex;align-items:center;justify-content:center;background:#13161f;border-radius:18px;font-size:5rem;color:#2a2e3a">🏨</div>`;
    return;
  }

  /* Asymmetric grid structure */
  container.style.cssText = `
    display: grid;
    grid-template-columns: 1fr 1fr;
    grid-template-rows: 240px 190px;
    gap: 6px;
    border-radius: 18px;
    overflow: hidden;
    margin-bottom: 44px;
  `;

  /* 1. MAIN (left, spans 2 rows) */
  const main = makeGalleryCell(photos[0], name, 'cell-main');
  main.style.cssText = `grid-column:1; grid-row:1/3; overflow:hidden; position:relative; background:#1a1e28; cursor:pointer;`;
  container.appendChild(main);

  /* 2. RIGHT TOP — split into 2 small images side by side */
  const rightTop = document.createElement('div');
  rightTop.style.cssText = `grid-column:2; grid-row:1; display:grid; grid-template-columns:1fr 1fr; gap:6px; overflow:hidden;`;

  const small1 = makeGalleryCell(photos[1] || '', name);
  small1.style.cssText = `overflow:hidden; position:relative; background:#1a1e28; cursor:pointer;`;
  const small2 = makeGalleryCell(photos[2] || '', name);
  small2.style.cssText = `overflow:hidden; position:relative; background:#1a1e28; cursor:pointer;`;
  rightTop.appendChild(small1);
  rightTop.appendChild(small2);
  container.appendChild(rightTop);

  /* 3. WIDE (right bottom) */
  const wide = makeGalleryCell(photos[3] || photos[0], name);
  wide.style.cssText = `grid-column:2; grid-row:2; overflow:hidden; position:relative; background:#1a1e28; cursor:pointer;`;

  /* Badge on wide if more photos */
  if(photos.length > 4) {
    const badge = document.createElement('div');
    badge.className = 'gallery-more-badge';
    badge.textContent = `+${photos.length - 4} photos`;
    wide.appendChild(badge);
  }
  container.appendChild(wide);

  /* Mobile override via media query already in CSS */
  applyMobileGallery(container, photos, name);
}

function makeGalleryCell(src, alt, cls='') {
  const cell = document.createElement('div');
  if(cls) cell.className = cls;
  if(src) {
    const img = document.createElement('img');
    img.src = src; img.alt = alt; img.loading = 'lazy';
    img.style.cssText = `width:100%;height:100%;object-fit:cover;transition:transform 0.55s cubic-bezier(0.4,0,0.2,1);display:block;`;
    img.onerror = () => { cell.innerHTML = `<div style="width:100%;height:100%;display:flex;align-items:center;justify-content:center;font-size:2.5rem;color:#2a2e3a">🏨</div>`; };
    cell.appendChild(img);
    cell.addEventListener('mouseenter', () => { img.style.transform='scale(1.07)'; });
    cell.addEventListener('mouseleave', () => { img.style.transform='scale(1)'; });
  } else {
    cell.innerHTML = `<div style="width:100%;height:100%;display:flex;align-items:center;justify-content:center;font-size:2.5rem;color:#2a2e3a;background:#13161f">🏨</div>`;
  }
  return cell;
}

function applyMobileGallery(container, photos, name) {
  const mq = window.matchMedia('(max-width: 700px)');
  const apply = (isMobile) => {
    if(isMobile) {
      container.style.cssText = `
        display:grid;
        grid-template-columns:1fr;
        grid-template-rows:240px auto auto 160px;
        gap:6px;
        border-radius:18px;
        overflow:hidden;
        margin-bottom:44px;
      `;
    }
  };
  mq.addEventListener('change', e => apply(e.matches));
  apply(mq.matches);
}

/* ══════════════════════════════════════════
   MARKET WIDGETS
══════════════════════════════════════════ */
async function loadFX() {
  const el = $('fxBody'); if(!el) return;
  try {
    const r = await fetch('https://api.exchangerate-api.com/v4/latest/TRY');
    const d = await r.json(); const rt = d.rates;
    const list = [
      {abbr:'US', code:'USD', name:'US Dollar'},
      {abbr:'EU', code:'EUR', name:'Euro'},
      {abbr:'RU', code:'RUB', name:'Russian Ruble'},
      {abbr:'GB', code:'GBP', name:'Pound Sterling'},
      {abbr:'JP', code:'JPY', name:'Japanese Yen'},
    ];
    el.innerHTML = list.map(c => {
      if(!rt[c.code]) return '';
      const rate = (1/rt[c.code]);
      const fmt  = c.code==='JPY'?rate.toFixed(6):rate.toFixed(4);
      return `<div class="fx-row">
        <div class="fx-left">
          <div class="fx-abbr">${c.abbr}</div>
          <div><div class="fx-code">${c.code}</div><div class="fx-name">${c.name}</div></div>
        </div>
        <div class="fx-rate">${fmt}</div>
      </div>`;
    }).join('');
    const ft = $('fxFoot'); if(ft) ft.textContent = `Updated ${ts()} · exchangerate-api.com`;
  } catch { if(el) el.innerHTML='<p class="mc-error">Could not load exchange rates.</p>'; }
}

async function loadFuel() {
  const el = $('fuelBody'); if(!el) return;
  try {
    const r = await fetch('https://api.exchangerate-api.com/v4/latest/EUR');
    const d = await r.json(); const eur = d.rates?.TRY || 36;
    const fuels = [
      {name:'Gasoline', sub:'RON 95',   eur:1.72, pct:84},
      {name:'Diesel',   sub:'Euro 5',   eur:1.58, pct:77},
      {name:'LPG',      sub:'Autogas',  eur:0.78, pct:38},
    ];
    el.innerHTML = fuels.map(f => `
      <div class="fuel-row">
        <div class="fuel-top">
          <div><div class="fuel-name">${f.name}</div><div class="fuel-type">${f.sub}</div></div>
          <div class="fuel-price">₺${(f.eur*eur).toFixed(2)}</div>
        </div>
        <div class="fuel-bar"><div class="fuel-fill" style="width:${f.pct}%"></div></div>
      </div>`).join('');
    const ft = $('fuelFoot'); if(ft) ft.textContent = `EUR/TRY conversion · ${ts()}`;
  } catch { if(el) el.innerHTML='<p class="mc-error">Could not load fuel data.</p>'; }
}

async function loadCrypto() {
  const el = $('cryptoBody'); if(!el) return;
  try {
    const ids = 'bitcoin,ethereum,solana,ripple,dogecoin,binancecoin';
    const r = await fetch(`https://api.coingecko.com/api/v3/simple/price?ids=${ids}&vs_currencies=usd&include_24hr_change=true`);
    const d = await r.json();
    const coins = [
      {id:'bitcoin',     name:'Bitcoin',  sym:'BTC',  cls:'c-btc',  ico:'₿'},
      {id:'ethereum',    name:'Ethereum', sym:'ETH',  cls:'c-eth',  ico:'Ξ'},
      {id:'solana',      name:'Solana',   sym:'SOL',  cls:'c-sol',  ico:'◎'},
      {id:'ripple',      name:'Ripple',   sym:'XRP',  cls:'c-xrp',  ico:'✕'},
      {id:'dogecoin',    name:'Dogecoin', sym:'DOGE', cls:'c-doge', ico:'Ð'},
      {id:'binancecoin', name:'BNB',      sym:'BNB',  cls:'c-bnb',  ico:'◆'},
    ];
    el.innerHTML = coins.map(c => {
      const cd = d[c.id]; if(!cd) return '';
      const p  = cd.usd;
      const chg = (cd.usd_24h_change||0).toFixed(2);
      const up = +chg >= 0;
      const ps = p>=1000?`$${fmtN(p,0)}`:p>=1?`$${p.toFixed(2)}`:`$${p.toFixed(5)}`;
      return `<div class="crypto-row">
        <div class="crypto-left">
          <div class="crypto-ico ${c.cls}">${c.ico}</div>
          <div><div class="crypto-name">${c.name}</div><div class="crypto-sym">${c.sym}</div></div>
        </div>
        <div class="crypto-right">
          <div class="crypto-price">${ps}</div>
          <div class="crypto-chg ${up?'chg-up':'chg-dn'}">${up?'▲':'▼'} ${Math.abs(chg)}%</div>
        </div>
      </div>`;
    }).join('');
    const ft = $('cryptoFoot'); if(ft) ft.textContent = `24h change · CoinGecko · ${ts()}`;
  } catch { if(el) el.innerHTML='<p class="mc-error">Could not load crypto data.</p>'; }
}

async function loadWeather() {
  const el = $('wxBody'); if(!el) return;
  try {
    const url = `https://api.open-meteo.com/v1/forecast?latitude=41.0082&longitude=28.9784&current=temperature_2m,relative_humidity_2m,apparent_temperature,weather_code,wind_speed_10m&timezone=Europe%2FIstanbul`;
    const r = await fetch(url); const data = await r.json(); const c = data.current;
    const temp   = Math.round(c.temperature_2m);
    const feels  = Math.round(c.apparent_temperature);
    const hum    = c.relative_humidity_2m;
    const wind   = Math.round(c.wind_speed_10m);
    const {emoji, desc} = wmoInfo(c.weather_code);
    el.innerHTML = `
      <div class="wx-city">Istanbul</div>
      <div class="wx-country">Turkey, TR</div>
      <div class="wx-main">
        <span class="wx-emoji">${emoji}</span>
        <span class="wx-temp">${temp}°<sup style="font-size:1.5rem">C</sup></span>
      </div>
      <div class="wx-cond">${desc}</div>
      <div class="wx-stats">
        <div class="wx-stat">
          <div class="wx-stat-ico">💧</div>
          <div class="wx-stat-val">${hum}%</div>
          <div class="wx-stat-lbl">Humidity</div>
        </div>
        <div class="wx-stat">
          <div class="wx-stat-ico">💨</div>
          <div class="wx-stat-val">${wind} km/h</div>
          <div class="wx-stat-lbl">Wind</div>
        </div>
        <div class="wx-stat">
          <div class="wx-stat-ico">🌡</div>
          <div class="wx-stat-val">${feels}°C</div>
          <div class="wx-stat-lbl">Feels Like</div>
        </div>
      </div>`;
    const ft = $('wxFoot'); if(ft) ft.textContent = `Open-Meteo · ${ts()}`;
  } catch { if(el) el.innerHTML='<p class="mc-error">Could not load weather.</p>'; }
}

function wmoInfo(code) {
  if(code===0)  return {emoji:'☀️', desc:'Clear sky'};
  if(code<=2)   return {emoji:'🌤', desc:'Partly cloudy'};
  if(code===3)  return {emoji:'☁️', desc:'Overcast'};
  if(code<=49)  return {emoji:'🌫', desc:'Foggy'};
  if(code<=57)  return {emoji:'🌧', desc:'Light drizzle'};
  if(code<=67)  return {emoji:'🌧', desc:'Rain'};
  if(code<=77)  return {emoji:'❄️', desc:'Snow'};
  if(code<=82)  return {emoji:'🌦', desc:'Rain showers'};
  if(code<=99)  return {emoji:'⛈', desc:'Thunderstorm'};
  return {emoji:'🌡', desc:'Unknown'};
}

/* ── Boot ── */
document.addEventListener('DOMContentLoaded', () => {
  initNavScroll();

  /* Index page */
  if($('searchBtn')) {
    initDates();
    initCity();
    loadFX();
    loadFuel();
    loadCrypto();
    loadWeather();
    setInterval(loadFX,     300000);
    setInterval(loadCrypto, 300000);
    setInterval(loadWeather,300000);
  }
});
