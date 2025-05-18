document.addEventListener('DOMContentLoaded', function() {
    // Hasta arama fonksiyonu
    const searchInput = document.getElementById('patientSearch');
    if (searchInput) {
        searchInput.addEventListener('input', function() {
            filterPatients(this.value.toLowerCase());
        });
    }

    function filterPatients(searchTerm) {
        const patientItems = document.querySelectorAll('.patient-item');
        
        patientItems.forEach(item => {
            const patientName = item.querySelector('.patient-name').textContent.toLowerCase();
            const patientId = item.querySelector('.patient-id').textContent.toLowerCase();
            
            if (patientName.includes(searchTerm) || patientId.includes(searchTerm)) {
                item.style.display = '';
            } else {
                item.style.display = 'none';
            }
        });
        
        // Sonuç bulunamadı mesajı
        const noResultsMsg = document.getElementById('noPatientResults');
        const visiblePatients = document.querySelectorAll('.patient-item[style="display: none;"]');
        
        if (visiblePatients.length === patientItems.length) {
            noResultsMsg.classList.remove('d-none');
        } else {
            noResultsMsg.classList.add('d-none');
        }
    }
    
    // Filtreleme
    const filterButtons = document.querySelectorAll('.btn-patient-filter');
    if (filterButtons.length) {
        filterButtons.forEach(btn => {
            btn.addEventListener('click', function() {
                // Aktif filtre butonunu güncelle
                filterButtons.forEach(b => b.classList.remove('active'));
                this.classList.add('active');
                
                const filterValue = this.dataset.filter;
                filterPatientsByStatus(filterValue);
            });
        });
    }
    
    function filterPatientsByStatus(status) {
        const patientItems = document.querySelectorAll('.patient-item');
        
        patientItems.forEach(item => {
            if (status === 'all') {
                item.style.display = '';
            } else {
                const patientStatus = item.dataset.status;
                
                if (patientStatus === status) {
                    item.style.display = '';
                } else {
                    item.style.display = 'none';
                }
            }
        });
    }
    
    // Hasta detaylarını görüntüleme (modal açma)
    const viewPatientLinks = document.querySelectorAll('.view-patient-btn');
    if (viewPatientLinks.length) {
        viewPatientLinks.forEach(link => {
            link.addEventListener('click', function(e) {
                e.preventDefault();
                const patientId = this.dataset.patientId;
                loadPatientDetails(patientId);
            });
        });
    }
    
    function loadPatientDetails(patientId) {
        // Gerçek uygulamada AJAX çağrısı ile API'den veri çekilir
        console.log(`Loading details for patient #${patientId}`);
        
        // Demo amaçlı manuel olarak modal içeriğini güncelleme
        document.getElementById('patientDetailsId').textContent = `#${patientId}`;
        
        // Modal gösterme
        const patientModal = new bootstrap.Modal(document.getElementById('patientDetailsModal'));
        patientModal.show();
    }
}); 